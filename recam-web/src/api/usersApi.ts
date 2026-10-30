import type { AxiosResponse } from "axios"
import apiClient from "./client"
import type { ApiResponse, PagedResult } from "../types/api"
import type { User } from "../types/users"

export const getAllUsers = async(pageNumber: number, pageSize: number):Promise<PagedResult<User>> => {
    const response : AxiosResponse<ApiResponse<PagedResult<User>>> = await apiClient.get<ApiResponse<PagedResult<User>>>('/users', {
            params: { pageNumber, pageSize },
        });
    return response.data.data
}