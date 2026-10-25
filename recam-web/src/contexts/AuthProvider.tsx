import { useState, type ReactNode } from "react";
import type { AuthUser, LoginRequest, LoginResponse } from "../types/auth";
import { login as loginRequest } from '../api/authApi';
import { AuthContext } from "./AuthContext";

interface AuthProviderProps{
    children: ReactNode
}

const AuthProvider = ({children}:AuthProviderProps) => {
    const [user,setUser] = useState<AuthUser| null>(()=>{
        const savedUser:string | null = localStorage.getItem('user');
        return savedUser ? (JSON.parse(savedUser) as AuthUser) : null;
    });

    const login = async(request:LoginRequest):Promise<void> =>{
        const result: LoginResponse = await loginRequest(request);
        const authUser: AuthUser = {
            userId: result.userId,
            email: result.email,
            role: result.role
        };
        localStorage.setItem('token',result.token);
        localStorage.setItem('user', JSON.stringify(authUser));
        setUser(authUser);
    };

    const logout = () :void => {
        localStorage.removeItem('token');
        localStorage.removeItem('user');
        setUser(null);
    };

    return (
        <AuthContext value={{user,login,logout}}>
            {children}
        </AuthContext>
    );
};

export default AuthProvider;