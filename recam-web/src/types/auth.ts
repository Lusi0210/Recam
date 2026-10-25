export type RoleName = "PhotographyCompany" | "Agent"

export interface LoginRequest {
    email:string,
    password:string
}

export interface LoginResponse{
    token:string,
    userId:string,
    email:string,
    role:RoleName
}

export interface AuthUser{
    userId:string,
    email:string,
    role:string
}

