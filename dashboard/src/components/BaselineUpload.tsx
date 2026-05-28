import React, { useState, useRef } from 'react';
import { UploadCloud, FileType, Loader2, CheckCircle, AlertCircle } from 'lucide-react';

interface BaselineUploadProps {
  token: string;
}

export default function BaselineUpload({ token }: BaselineUploadProps) {
  const [file, setFile] = useState<File | null>(null);
  const [status, setStatus] = useState<'idle' | 'uploading' | 'success' | 'error'>('idle');
  const [errorMessage, setErrorMessage] = useState('');
  const fileInputRef = useRef<HTMLInputElement>(null);

  const handleFileChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    if (e.target.files && e.target.files.length > 0) {
      setFile(e.target.files[0]);
      setStatus('idle');
      setErrorMessage('');
    }
  };

  const handleUpload = async () => {
    if (!file) return;

    setStatus('uploading');
    
    const formData = new FormData();
    formData.append('file', file);
    
    const cleanToken = token.replace(/['"]+/g, '').trim();

    try {
      const response = await fetch('http://localhost:5110/api/baseline/upload', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${cleanToken}`
        },
        body: formData
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`[Status ${response.status}] ${errorText || response.statusText}`);
      }

      setStatus('success');
      setTimeout(() => {
        setFile(null);
        setStatus('idle');
        if (fileInputRef.current) fileInputRef.current.value = '';
      }, 2000);

    } catch (err: any) {
      setStatus('error');
      setErrorMessage(err.message || 'Upload failed.');
    }
  };

  return (
    <div className="bg-slate-900 border border-slate-800 rounded-xl p-6 shadow-lg">
      <h3 className="text-lg font-semibold text-white mb-4">Upload Baseline Data</h3>
      
      <div className="flex flex-col items-center justify-center border-2 border-dashed border-slate-700 rounded-lg p-8 bg-slate-950/50 hover:bg-slate-900/80 transition-colors">
        
        <input 
          type="file" 
          ref={fileInputRef}
          onChange={handleFileChange} 
          className="hidden" 
          accept=".bin,.dat,.txt,.csv"
        />

        {!file ? (
          <div className="text-center cursor-pointer" onClick={() => fileInputRef.current?.click()}>
            <UploadCloud className="w-12 h-12 text-indigo-400 mx-auto mb-3" />
            <p className="text-slate-300 font-medium">Click to select a file</p>
            <p className="text-slate-500 text-sm mt-1">Supports binary or text data files</p>
          </div>
        ) : (
          <div className="text-center w-full">
            <FileType className="w-12 h-12 text-indigo-400 mx-auto mb-3" />
            <p className="text-slate-200 font-medium truncate px-4">{file.name}</p>
            <p className="text-slate-500 text-sm mt-1">{(file.size / 1024).toFixed(2)} KB</p>
            
            {status === 'idle' && (
              <div className="mt-6 flex gap-3 justify-center">
                <button 
                  onClick={() => setFile(null)}
                  className="px-4 py-2 rounded-lg text-slate-400 hover:text-white border border-slate-700 hover:bg-slate-800 transition-colors"
                >
                  Cancel
                </button>
                <button 
                  onClick={handleUpload}
                  className="px-6 py-2 rounded-lg bg-indigo-600 text-white font-medium hover:bg-indigo-700 transition-colors shadow-lg shadow-indigo-500/20"
                >
                  Upload & Analyze
                </button>
              </div>
            )}

            {status === 'uploading' && (
              <div className="mt-6 flex items-center justify-center gap-2 text-indigo-400">
                <Loader2 className="w-5 h-5 animate-spin" />
                <span>Uploading to engine...</span>
              </div>
            )}

            {status === 'success' && (
              <div className="mt-6 flex items-center justify-center gap-2 text-emerald-400">
                <CheckCircle className="w-5 h-5" />
                <span>File uploaded and processed successfully!</span>
              </div>
            )}

            {status === 'error' && (
              <div className="mt-6 flex flex-col items-center gap-2 text-red-400">
                <div className="flex items-center gap-2">
                  <AlertCircle className="w-5 h-5 flex-shrink-0" />
                  <span className="text-left break-all">{errorMessage}</span>
                </div>
                <button onClick={() => setStatus('idle')} className="text-sm underline mt-1 text-slate-400 hover:text-white">Try Again</button>
              </div>
            )}
          </div>
        )}
      </div>
    </div>
  );
}