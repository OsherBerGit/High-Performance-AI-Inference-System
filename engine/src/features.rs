pub fn calculate_shannon_entropy(data: &[u8]) -> f64 {
    if data.is_empty() {
        return 0.0;
    }

    let mut frequencies = [0usize; 256];

    for &byte in data {
        frequencies[byte as usize] += 1;
    }

    let total_bytes = data.len() as f64;
    let mut entropy = 0.0;
    for &freq in &frequencies {
        if freq > 0 {
            let probability = freq as f64 / total_bytes;
            entropy -= probability * probability.log2();
        }
    }

    entropy
}

pub fn calculate_eccentricity(data: &[u8]) -> f64 {
    if data.is_empty() { return 0.0; }

    let width = (data.len() as f64).sqrt() as usize;
    if width * width != data.len() {
        return 0.0;
    }

    let mut m00 = 0.0;
    let mut m10 = 0.0;
    let mut m01 = 0.0;

    for (i, &val) in data.iter().enumerate() {
        let val_f = val as f64;
        if val_f > 0.0 {
            let x = (i % width) as f64;
            let y = (i / width) as f64;
            m00 += val_f;
            m10 += x * val_f;
            m01 += y * val_f;
        }
    }

    if m00 == 0.0 { return 0.0; }

    let cx = m10 / m00;
    let cy = m01 / m00;
    
    let mut mu20 = 0.0;
    let mut mu02 = 0.0;
    let mut mu11 = 0.0;

    for (i, &val) in data.iter().enumerate() {
        let val_f = val as f64;
        if val_f > 0.0 {
            let x = (i % width) as f64;
            let y = (i / width) as f64;
            let dx = x - cx;
            let dy = y - cy;
            mu20 += (dx * dx) * val_f;
            mu02 += (dy * dy) * val_f;
            mu11 += (dx * dy) * val_f;
        }
    }

    if mu20 == 0.0 { return 0.0; }
    
    mu20 /= m00;
    mu02 /= m00;
    mu11 /= m00;

    let base_term = mu20 + mu02;
    let root_term = ((mu20 - mu02).powi(2) + 4.0 * mu11.powi(2)).sqrt();
    let lambda1: f64 = (base_term + root_term) / 2.0;
    let lambda2: f64 = (base_term - root_term) / 2.0;

    if lambda1 == 0.0 { return 0.0; }

    let e = (1.0 - (lambda2 / lambda1)).sqrt();
    
    e
}

pub fn calculate_confidence(data: &[u8], baseline: &[u8]) -> f64 {
    if data.is_empty() || baseline.is_empty() || data.len() != baseline.len() {
        return 0.0;
    }

    let dot_product: f64 = data.iter().zip(baseline.iter()).map(|(&a, &b)| (a as f64) * (b as f64)).sum();
    let magnitude_a: f64 = data.iter().map(|&a| (a as f64).powi(2)).sum::<f64>().sqrt();
    let magnitude_b: f64 = baseline.iter().map(|&b| (b as f64).powi(2)).sum::<f64>().sqrt();

    if magnitude_a == 0.0 || magnitude_b == 0.0 {
        return 0.0;
    }

    let confidence = dot_product / (magnitude_a * magnitude_b);

    confidence
}

pub fn calculate_standard_deviation(data: &[u8]) -> f64 {
    if data.is_empty() { return 0.0; }
    
    let mean = data.iter().map(|&x| x as f64).sum::<f64>() / (data.len() as f64);
    let variance = data.iter().map(|&x| {
        let diff = x as f64 - mean;
        diff * diff
    }).sum::<f64>() / (data.len() as f64);
    
    variance.sqrt()
}

pub fn calculate_mean_absolute_deviation(data: &[u8]) -> f64 {
    if data.is_empty() { return 0.0; }
    
    let mean = data.iter().map(|&x| x as f64).sum::<f64>() / (data.len() as f64);
    let mad = data.iter().map(|&x| (x as f64 - mean).abs()).sum::<f64>() / (data.len() as f64);
    
    mad
}

pub fn calculate_peak_to_average_ratio(data: &[u8]) -> f64 {
    if data.is_empty() { return 0.0; }
    
    let peak = data.iter().copied().max().unwrap_or(0) as f64;
    let mean = data.iter().map(|&x| x as f64).sum::<f64>() / (data.len() as f64);
    
    if mean == 0.0 {
        return 0.0;
    }
    
    peak / mean
}