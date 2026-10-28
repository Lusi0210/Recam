import type { AxiosResponse } from "axios";
import type { ApiResponse, PagedResult } from "../types/api";
import type { CreateListingCaseRequest, CreateListingCaseResponse, ListingCase, ListingCaseDetailsResponse, ListingCaseQuery } from "../types/listingCase";
import apiClient from "./client";

export const getListingCases = async (query : ListingCaseQuery): Promise<PagedResult<ListingCase>> => {
    const response:AxiosResponse<ApiResponse<PagedResult<ListingCase>>> = await apiClient.get<ApiResponse<PagedResult<ListingCase>>>('/listingcase',{ params: query})
    return response.data.data;
}

export const createListingCase = async (request: CreateListingCaseRequest) : Promise<CreateListingCaseResponse> => {
    const response:AxiosResponse<ApiResponse<CreateListingCaseResponse>> = await apiClient.post<ApiResponse<CreateListingCaseResponse>>('/listingcase',request);
    return response.data.data;
}

export const getListingCaseById = async (id:number) : Promise<ListingCaseDetailsResponse> => {
    const response:AxiosResponse<ApiResponse<ListingCaseDetailsResponse>> = await apiClient.get<ApiResponse<ListingCaseDetailsResponse>>(`/listingcase/${id}`);
    return response.data.data;
}