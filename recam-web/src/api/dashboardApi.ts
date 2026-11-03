import type { AxiosResponse } from "axios";
import type { ApiResponse } from "../types/api";
import type { DashboardResponse } from "../types/dashboard";
import apiClient from "./client";

export const getDashboard = async (): Promise<DashboardResponse> => {
    const response: AxiosResponse<ApiResponse<DashboardResponse>> = await apiClient.get<ApiResponse<DashboardResponse>>('/dashboard');
    return response.data.data;
}