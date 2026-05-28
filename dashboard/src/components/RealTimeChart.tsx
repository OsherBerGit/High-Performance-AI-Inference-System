import React, { useEffect, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { Activity, Wifi, WifiOff, AlertTriangle, Play, Loader2 } from 'lucide-react';

interface RealTimeChartProps {
  token: string;
}

interface TelemetryData {
  time: string;
  entropy: number;
  eccentricity: number;
}

interface InferenceResultDto {
  requestId: string;
  entropy: number;
  eccentricity: number;
  confidence: number;
  flagged: boolean;
}

export default function RealTimeChart({ token }: RealTimeChartProps) {
  const [data, setData] = useState<TelemetryData[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const [anomalyDetected, setAnomalyDetected] = useState(false);
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5110/hubs/inference', {
        accessTokenFactory: () => {
          return token.replace(/['"]+/g, '').trim();
        }
      })
      .configureLogging(LogLevel.Information)
      .withAutomaticReconnect()
      .build();

    connection.start()
      .then(() => {
        setIsConnected(true);
        connection.on('receiveinferenceupdate', (result: InferenceResultDto) => {
          const now = new Date().toLocaleTimeString();
          setData(prev => {
            // Fallback for PascalCase in case SignalR serialization defaults were changed
            const entropy = result.entropy ?? (result as any).Entropy ?? 0;
            const eccentricity = result.eccentricity ?? (result as any).Eccentricity ?? 0;
            const newData = [...prev, { time: now, entropy, eccentricity }];
            return newData.slice(-20);
          });
          setAnomalyDetected(result.flagged ?? (result as any).Flagged ?? false);
        });
      })
      .catch(() => setIsConnected(false));

    return () => {
      connection.stop();
    };
  }, [token]);

  const startAnalysis = async () => {
    setIsAnalyzing(true);
    setError('');
    const cleanToken = token.replace(/['"]+/g, '').trim();

    try {
      const response = await fetch('http://localhost:5110/api/inference/analyze', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${cleanToken}`
        }
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`[Status ${response.status}] ${errorText || response.statusText}`);
      }
    } catch (err: any) {
      setError(err.message || 'Error starting inference engine.');
    } finally {
      setTimeout(() => setIsAnalyzing(false), 2000);
    }
  };

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-lg col-span-1 md:col-span-2 mt-6">
      <div className="flex justify-between items-center mb-6">
        <div className="flex items-center gap-3">
          <Activity className="w-6 h-6 text-indigo-400" />
          <h3 className="text-lg font-semibold text-white">Live Inference Telemetry</h3>
        </div>
        
        <div className="flex items-center gap-4">
          {error && <span className="text-red-400 text-sm">{error}</span>}
          
          <button
            onClick={startAnalysis}
            disabled={!isConnected || isAnalyzing}
            className="flex items-center gap-2 px-4 py-2 bg-indigo-600 hover:bg-indigo-700 disabled:bg-slate-800 disabled:text-slate-500 text-white rounded-lg transition-colors font-medium text-sm shadow-lg shadow-indigo-500/20"
          >
            {isAnalyzing ? <Loader2 className="w-4 h-4 animate-spin" /> : <Play className="w-4 h-4" />}
            {isAnalyzing ? 'Starting...' : 'Start Analysis'}
          </button>

          {anomalyDetected && (
            <span className="flex items-center gap-1 text-red-400 text-sm font-medium animate-pulse">
              <AlertTriangle className="w-4 h-4" /> Anomaly Detected
            </span>
          )}
          <span className={`flex items-center gap-1 text-sm font-medium ${isConnected ? 'text-emerald-400' : 'text-slate-500'}`}>
            {isConnected ? <Wifi className="w-4 h-4" /> : <WifiOff className="w-4 h-4" />}
            {isConnected ? 'Connected' : 'Disconnected'}
          </span>
        </div>
      </div>

      <div className="h-72 w-full">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={data} margin={{ top: 5, right: 20, bottom: 5, left: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#1e293b" />
            <XAxis dataKey="time" stroke="#64748b" fontSize={12} tickMargin={10} />
            <YAxis stroke="#64748b" fontSize={12} />
            <Tooltip 
              contentStyle={{ backgroundColor: '#0f172a', borderColor: '#1e293b', color: '#f1f5f9' }}
              itemStyle={{ color: '#818cf8' }}
            />
            <Legend />
            <Line type="monotone" dataKey="entropy" stroke="#818cf8" strokeWidth={2} dot={true} animationDuration={300} isAnimationActive={false} />
            <Line type="monotone" dataKey="eccentricity" stroke="#34d399" strokeWidth={2} dot={true} animationDuration={300} isAnimationActive={false} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}