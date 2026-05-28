import { useState } from 'react';
import Login from './components/Login';
import BaselineUpload from './components/BaselineUpload';
import RealTimeChart from './components/RealTimeChart';

export default function App() {
  const [token, setToken] = useState<string | null>(localStorage.getItem('jwt_token'));

  const handleLoginSuccess = (newToken: string) => {
    localStorage.setItem('jwt_token', newToken);
    setToken(newToken);
  };

  const handleLogout = () => {
    localStorage.removeItem('jwt_token');
    setToken(null);
  };

  if (!token)
    return <Login onLoginSuccess={handleLoginSuccess} />;

  return (
    <div className="min-h-screen bg-slate-950 text-slate-200 p-8 flex flex-col items-center">
      <header className="w-full max-w-4xl flex justify-between items-center mb-8 border-b border-slate-800 pb-4">
        <h1 className="text-3xl font-bold text-white tracking-tight">Telemetry Dashboard</h1>
        <button 
          onClick={handleLogout}
          className="px-4 py-2 text-sm text-slate-400 hover:text-white bg-slate-900 border border-slate-800 hover:border-slate-700 rounded transition-colors"
        >
          Logout
        </button>
      </header>
      
      <main className="w-full max-w-4xl">
        <BaselineUpload token={token} />
        <RealTimeChart token={token} />
      </main>
    </div>
  );
}