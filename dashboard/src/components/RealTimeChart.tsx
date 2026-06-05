import React, { useEffect, useState } from 'react';
import { HubConnectionBuilder, LogLevel } from '@microsoft/signalr';
import { LineChart, Line, XAxis, YAxis, CartesianGrid, Tooltip, ResponsiveContainer, Legend } from 'recharts';
import { Activity, Wifi, WifiOff, AlertTriangle, Play, Loader2, Server } from 'lucide-react';

interface RealTimeChartProps {
  token: string;
}

interface TelemetryData {
  time: string;
  entropy: number;
  eccentricity: number;
  stdDev: number;
  mad: number;
  par: number;
}

interface InferenceResultDto {
  requestId: string;
  entropy: number;
  eccentricity: number;
  confidence: number;
  flagged: boolean;
  standardDeviation: number;
  meanAbsoluteDeviation: number;
  peakToAverageRatio: number;
}

export default function RealTimeChart({ token }: RealTimeChartProps) {
  const [data, setData] = useState<TelemetryData[]>([]);
  const [isConnected, setIsConnected] = useState(false);
  const [engineStatus, setEngineStatus] = useState<'checking' | 'healthy' | 'down'>('checking');
  const [anomalyDetected, setAnomalyDetected] = useState(false);
  const [isAnalyzing, setIsAnalyzing] = useState(false);
  const [error, setError] = useState('');

  useEffect(() => {
    const connection = new HubConnectionBuilder()
      .withUrl('http://localhost:5110/hubs/inference', {
        accessTokenFactory: () => token.replace(/['"]+/g, '').trim()
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
            const newData = [...prev, { 
              time: now, 
              entropy: result.entropy, 
              eccentricity: result.eccentricity,
              stdDev: result.standardDeviation,
              mad: result.meanAbsoluteDeviation,
              par: result.peakToAverageRatio
            }];
            return newData.slice(-20);
          });
          setAnomalyDetected(result.flagged);
        });
      })
      .catch(() => setIsConnected(false));

    return () => {
      connection.stop();
    };
  }, [token]);

  useEffect(() => {
    const checkEngineHealth = async () => {
      try {
        const cleanToken = token.replace(/['"]+/g, '').trim();
        const response = await fetch('http://localhost:5110/api/health/engine', {
          headers: { 'Authorization': `Bearer ${cleanToken}` }
        });
        setEngineStatus(response.ok ? 'healthy' : 'down');
      } catch {
        setEngineStatus('down');
      }
    };

    checkEngineHealth();
    const interval = setInterval(checkEngineHealth, 5000);
    return () => clearInterval(interval);
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
      <div className="flex flex-col md:flex-row justify-between items-start md:items-center mb-6 gap-4">
        <div className="flex items-center gap-3">
          <Activity className="w-6 h-6 text-indigo-400" />
          <h3 className="text-lg font-semibold text-white">Live Inference Telemetry</h3>
        </div>
        
        <div className="flex flex-wrap items-center gap-4">
          {/* Rust Engine Status */}
          <div className="flex items-center gap-2 bg-slate-800 px-3 py-1.5 rounded-lg border border-slate-700">
            <Server className="w-4 h-4 text-slate-400" />
            <span className="text-xs font-medium text-slate-300">Engine:</span>
            {engineStatus === 'checking' && <Loader2 className="w-3 h-3 text-slate-400 animate-spin" />}
            {engineStatus === 'healthy' && <span className="w-2 h-2 rounded-full bg-emerald-500 animate-pulse" />}
            {engineStatus === 'down' && <span className="w-2 h-2 rounded-full bg-red-500" />}
          </div>

          {error && <span className="text-red-400 text-sm">{error}</span>}
          
          <button
            onClick={startAnalysis}
            disabled={!isConnected || isAnalyzing || engineStatus !== 'healthy'}
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
          
          {/* SignalR Status */}
          <span className={`flex items-center gap-1 text-sm font-medium ${isConnected ? 'text-emerald-400' : 'text-slate-500'}`}>
            {isConnected ? <Wifi className="w-4 h-4" /> : <WifiOff className="w-4 h-4" />}
            {isConnected ? 'Gateway Connected' : 'Gateway Offline'}
          </span>
        </div>
      </div>

      <div className="h-96 w-full">
        <ResponsiveContainer width="100%" height="100%">
          <LineChart data={data} margin={{ top: 5, right: 20, bottom: 5, left: 0 }}>
            <CartesianGrid strokeDasharray="3 3" stroke="#1e293b" />
            <XAxis dataKey="time" stroke="#64748b" fontSize={12} tickMargin={10} />
            <YAxis stroke="#64748b" fontSize={12} />
            <Tooltip 
              contentStyle={{ backgroundColor: '#0f172a', borderColor: '#1e293b', color: '#f1f5f9' }}
              itemStyle={{ color: '#818cf8' }}
            />
            <Legend wrapperStyle={{ paddingTop: '20px' }} />
            <Line type="monotone" dataKey="entropy" stroke="#818cf8" strokeWidth={2} dot={true} />
            <Line type="monotone" dataKey="eccentricity" stroke="#34d399" strokeWidth={2} dot={true} />
            <Line type="monotone" dataKey="stdDev" name="Std Dev" stroke="#f472b6" strokeWidth={2} dot={true} />
            <Line type="monotone" dataKey="mad" name="MAD" stroke="#fbbf24" strokeWidth={2} dot={true} />
            <Line type="monotone" dataKey="par" name="PAR" stroke="#38bdf8" strokeWidth={2} dot={true} />
          </LineChart>
        </ResponsiveContainer>
      </div>
    </div>
  );
}