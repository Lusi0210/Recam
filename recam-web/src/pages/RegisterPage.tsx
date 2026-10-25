import { useState, type SyntheticEvent } from "react"
import type { RegisterRequest } from "../types/auth";
import { register } from "../api/authApi";
import { Link, useNavigate } from "react-router-dom";
import axios from "axios";
import type { ApiResponse } from "../types/api";

const RegisterPage = () =>{
    const [email,setEmail] = useState<string>('');
    const [password,setPassword] = useState<string>('');
    const [confirmPassword,setConfirmPassword] = useState<string>('');
    const [agentFirstName,setAgentFirstName] = useState<string>('');
    const [agentLastName,setAgentLastName] = useState<string>('');
    const [companyName,setCompanyName] = useState<string>('');
    const navigate = useNavigate();
    const [errorMessage,setErrorMessage] = useState<string>('');
    const [isLoading,setIsLoading] = useState<boolean>(false);
    

    const handleSubmit = async (event:SyntheticEvent<HTMLFormElement>):Promise<void> => {
        event.preventDefault();
        setErrorMessage('');

        if (password !== confirmPassword) {
            setErrorMessage('Passwords do not match');
            return;
        }

        setIsLoading(true);
        const request:RegisterRequest = {email,password,confirmPassword,agentFirstName,agentLastName,companyName};
        try {
            await register(request);
            navigate('/login')
        } catch (error: unknown) {
            if (axios.isAxiosError<ApiResponse<null>>(error)) {
                const errors: string[] | null | undefined = error.response?.data?.errors;
                if (errors && errors.length > 0) {
                    setErrorMessage(errors.join(', '));
                } else {
                    setErrorMessage(error.response?.data?.message ?? 'Registration failed');
                }
            } else {
                setErrorMessage('Something went wrong');
            }
        }
        finally{
            setIsLoading(false);
        } 
        
    }

    return (
        <div className="min-h-screen flex items-center justify-center bg-gray-100">
            <form
            onSubmit={handleSubmit}
            className="w-full max-w-sm bg-white p-8 rounded-lg shadow-md flex flex-col gap-4"
            >
            <h1 className="text-2xl font-bold text-center">Register</h1>

            {errorMessage && (
                <p className="text-sm text-red-600 bg-red-50 p-2 rounded">{errorMessage}</p>
            )}

            <input type="email" value={email} onChange={(e) => setEmail(e.target.value)}
                placeholder="Email" required
                className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <input type="password" value={password} onChange={(e) => setPassword(e.target.value)}
                placeholder="Password" autoComplete="new-password" required
                className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            <input type="password" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)}
                placeholder="Confirm password" autoComplete="new-password" required
                className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />

            <div className="flex gap-2">
                <input type="text" value={agentFirstName} onChange={(e) => setAgentFirstName(e.target.value)}
                placeholder="First name" required
                className="w-1/2 border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
                <input type="text" value={agentLastName} onChange={(e) => setAgentLastName(e.target.value)}
                placeholder="Last name" required
                className="w-1/2 border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />
            </div>

            <input type="text" value={companyName} onChange={(e) => setCompanyName(e.target.value)}
                placeholder="Company name"
                className="border border-gray-300 rounded px-3 py-2 focus:outline-none focus:ring-2 focus:ring-blue-500" />

            <button type="submit" disabled={isLoading}
                className="bg-blue-600 text-white rounded py-2 font-medium hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed">
                {isLoading ? 'Registering...' : 'Register'}
            </button>

            <p className="text-sm text-center text-gray-600">
                Already have an account?{' '}
                <Link to="/login" className="text-blue-600 hover:underline">Login</Link>
            </p>
            </form>
        </div>
        );
}

export default RegisterPage;