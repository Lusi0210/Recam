import type { AxiosResponse } from "axios";
import type { LoginRequest, LoginResponse, RegisterRequest, RegisterResponse } from "../types/auth";
import apiClient from "./client";
import type { ApiResponse } from "../types/api";

export const login = async (request:LoginRequest) :Promise<LoginResponse> => {
    const response:AxiosResponse<ApiResponse<LoginResponse>> = await apiClient.post<ApiResponse<LoginResponse>>('/auth/login',request)
    return  response.data.data
}

export const register = async (request:RegisterRequest) : Promise<RegisterResponse> => {
    const response:AxiosResponse<ApiResponse<RegisterResponse>> = await apiClient.post<ApiResponse<RegisterResponse>>('/auth/register',request);
    return response.data.data
}
