import { useState, type SyntheticEvent } from "react"
import type { ApiResponse } from "../types/api";
import type { LoginRequest } from "../types/auth";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

export default function LoginPage() {

  const [email,setEmail] = useState<string>('');
  const [password,setPassword] = useState<string>('');
  const [errorMessage,setErrorMessage] = useState<string>('');
  const navigate = useNavigate();
  const [isLoading,setIsLoading] = useState<boolean>(false);
  const authContext = useAuth();



  const  handleSubmit = async (event:SyntheticEvent<HTMLFormElement>) :Promise<void> =>{
    event.preventDefault();
    setErrorMessage('');
    setIsLoading(true);
    const request:LoginRequest = {email,password};
    try {
      await authContext.login(request);
      navigate('/')
    } catch(error: unknown){
      if(axios.isAxiosError<ApiResponse<null>>(error)){
        setErrorMessage(error.response?.data?.message ?? 'Login failed')
      } else {
        setErrorMessage('Something went wrong')
      }
    }
    finally{
      setIsLoading(false);
    }
  }



  return<div className="min-h-screen flex items-center justify-center bg-gray-100">
          <form
            onSubmit={handleSubmit}
            className="w-full max-w-sm bg-white p-8 rounded-lg shadow-md flex flex-col gap-4"
          >
            <h1 className="text-2xl font-bold text-center">Login</h1>

            {errorMessage && (
              <p className="text-sm text-red-600 bg-red-50 p-2 rounded">{errorMessage}</p>
            )}

            <input
              type="email"
              placeholder="Enter your email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
              type="password"
              placeholder="Enter your password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500"
            />

            <button
              type="submit"
              disabled={isLoading}
              className="bg-blue-600 text-white rounded py-2 font-medium hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed"
            >
              {isLoading ? 'Logging in...' : 'Login'}
            </button>
          </form>
        </div>
}