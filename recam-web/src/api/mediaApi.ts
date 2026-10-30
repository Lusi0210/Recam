import type { AxiosResponse } from "axios";
import type { MediaAsset, MediaType } from "../types/mediaAsset"
import apiClient from "./client"
import type { ApiResponse } from "../types/api";

export const uploadMedia  = async (listingcaseId:number,files: File[],mediaType:MediaType,onProgress?:(percent:number)=> void) : Promise<MediaAsset[]> => {
    const formData:FormData = new FormData();
    files.forEach((file) => formData.append('files',file));
    formData.append('mediaType',String(mediaType));

    const response: AxiosResponse<ApiResponse<MediaAsset[]>> =  
        await apiClient.post<ApiResponse<MediaAsset[]>>(
            `/listingcase/${listingcaseId}/media`,
            formData,
        {
            onUploadProgress:(event)=>{
                if(event.total && onProgress){
                    onProgress(Math.round((event.loaded *100)/event.total));
                }
            },
        });
    return response.data.data;
}
