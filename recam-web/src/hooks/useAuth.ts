import { useContext } from "react"
import { AuthContext, type AuthContextType } from "../contexts/AuthContext"

export const useAuth = ():AuthContextType => {
    const context:AuthContextType | null = useContext(AuthContext);
    if(!context){
        throw new Error('useAuth must be used within AuthProvider');
    }
    return context;
}