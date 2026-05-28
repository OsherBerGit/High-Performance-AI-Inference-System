use computation::engine_service_server::{EngineService, EngineServiceServer};
use computation::{FeatureRequest, FeatureResponse};
use std::sync::{Arc, Mutex};
use tonic::{transport::Server, Request, Response, Status};

pub mod computation {
    tonic::include_proto!("computation");
}

pub mod features;

#[derive(Debug, Default)]
pub struct EngineState {
    pub total_requests_processed: u64,
}

#[derive(Debug, Default)]
pub struct HybridEngine {
    pub state: Arc<Mutex<EngineState>>,
}

#[tonic::async_trait]
impl EngineService for HybridEngine {
    async fn compute_features(
        &self,
        request: Request<FeatureRequest>,
    ) -> Result<Response<FeatureResponse>, Status> {
        let mut state = self.state.lock().unwrap();
        state.total_requests_processed += 1;
        println!("Total requests processed: {}",state.total_requests_processed);

        drop(state);

        let inner_req = request.into_inner();
        println!("Received request with ID: {}", inner_req.request_id);
        println!("Received raw data size: {} bytes", inner_req.raw_data.len());

        let entropy = features::calculate_shannon_entropy(&inner_req.raw_data);

        let dummy_baseline = vec![255u8; inner_req.raw_data.len()]; 
        let confidence = features::calculate_confidence(&inner_req.raw_data, &dummy_baseline);

        let mut raw_data = vec![0u8; 100];
        raw_data[44] = 255;
        raw_data[45] = 255;
        raw_data[55] = 255;
        let eccentricity = features::calculate_eccentricity(&raw_data);

        let reply = FeatureResponse {
            request_id: inner_req.request_id,
            shannon_entropy: entropy,
            eccentricity: eccentricity,
            confidence_score: confidence,
            error_message: String::new(),
        };

        Ok(Response::new(reply))
    }
}

#[tokio::main]
async fn main() -> Result<(), Box<dyn std::error::Error>> {
    let addr = "[::1]:50051".parse()?;
    let engine = HybridEngine::default();

    println!("Hybrid AI Engine listening on {}", addr);

    Server::builder()
        .add_service(EngineServiceServer::new(engine))
        .serve(addr)
        .await?;

    Ok(())
}

#[cfg(test)]
mod tests {
    use super::*;
    use std::collections::HashMap;

    fn compute_shannon_entropy(data: &[u8]) -> f64 {
        if data.is_empty() { return 0.0; }
        let mut counts = HashMap::new();
        for &b in data {
            *counts.entry(b).or_insert(0) += 1;
        }
        let len = data.len() as f64;
        counts.values().fold(0.0, |acc, &count| {
            let p = count as f64 / len;
            acc - p * p.log2()
        })
    }

    #[test]
    fn test_zero_entropy_for_uniform_data() {
        let data = vec![0, 0, 0, 0, 0];
        let entropy = compute_shannon_entropy(&data);
        assert_eq!(entropy, 0.0);
    }

    #[test]
    fn test_high_entropy_for_diverse_data() {
        let data = vec![1, 2, 3, 4];
        let entropy = compute_shannon_entropy(&data);
        assert_eq!(entropy, 2.0);
    }

    #[test]
    fn test_empty_payload_handling() {
        let data: Vec<u8> = vec![];
        let entropy = compute_shannon_entropy(&data);
        assert_eq!(entropy, 0.0);
    }
}
