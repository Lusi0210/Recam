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
    role:RoleName
}

export interface RegisterRequest{
    email:string,
    password:string,
    confirmPassword:string,
    agentFirstName:string,
    agentLastName:string,
    companyName:string | null
}

export interface RegisterResponse{
    userId:string,
    email:string,
    firstName:string,
    lastName:string,
}

