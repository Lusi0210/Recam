import axios from "axios";
import type { ApiResponse } from "../types/api";

export const getErrorMessage = (error: unknown, fallback: string): string => {
    if (!axios.isAxiosError<ApiResponse<null>>(error)) {
        return 'Something went wrong';
    }

    const errors: string[] | null | undefined = error.response?.data?.errors;
    if (errors && errors.length > 0) {
        return errors.join(', ');
    }

    return error.response?.data?.message ?? fallback;
};