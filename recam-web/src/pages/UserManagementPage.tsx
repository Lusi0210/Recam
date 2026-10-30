import { useEffect, useState } from "react";
import axios from "axios";
import { getAllUsers } from "../api/usersApi";
import type { ApiResponse, PagedResult } from "../types/api";
import type { User } from "../types/users";

const PAGE_SIZE: number = 2;

const UserManagementPage = () => {
    const [pageNumber, setPageNumber] = useState<number>(1);
    const [result, setResult] = useState<PagedResult<User> | null>(null);
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorMessage, setErrorMessage] = useState<string>('');

    useEffect(() => {
        const fetchData = async (): Promise<void> => {
            setIsLoading(true);
            setErrorMessage('');
            try {
                const response: PagedResult<User> = await getAllUsers(pageNumber, PAGE_SIZE);
                setResult(response);
            } catch (error: unknown) {
                if (axios.isAxiosError<ApiResponse<null>>(error)) {
                    const errors: string[] | null | undefined = error.response?.data?.errors;
                    if (errors && errors.length > 0) {
                        setErrorMessage(errors.join(', '));
                    } else {
                        setErrorMessage(error.response?.data?.message ?? 'Failed to load users');
                    }
                } else {
                    setErrorMessage('Something went wrong');
                }
            } finally {
                setIsLoading(false);
            }
        };
        fetchData();
    }, [pageNumber]);

    const users: User[] = result?.items ?? [];

    return (
        <div>
            <h1 className="text-2xl font-bold mb-4">User Management</h1>

            {errorMessage && (
                <p className="mb-4 text-sm text-red-600 bg-red-50 p-3 rounded">{errorMessage}</p>
            )}

            <div className="bg-white rounded-lg shadow overflow-x-auto">
                <table className="min-w-full text-sm">
                    <thead className="bg-gray-50 text-left text-gray-600">
                        <tr>
                            <th className="px-4 py-3 font-medium">Email</th>
                            <th className="px-4 py-3 font-medium">Role</th>
                            <th className="px-4 py-3 font-medium">Joined</th>
                        </tr>
                    </thead>
                    <tbody className="divide-y divide-gray-100">
                        {isLoading ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-8 text-center text-gray-500">Loading...</td>
                            </tr>
                        ) : users.length === 0 ? (
                            <tr>
                                <td colSpan={3} className="px-4 py-8 text-center text-gray-500">No users found</td>
                            </tr>
                        ) : (
                            users.map((user) => (
                                <tr key={user.id} className="hover:bg-gray-50">
                                    <td className="px-4 py-3 font-medium text-gray-900">{user.email}</td>
                                    <td className="px-4 py-3">
                                        <span className={`px-2 py-1 rounded-full text-xs font-medium ${
                                            user.role === 'PhotographyCompany' ? 'bg-purple-100 text-purple-800' : 'bg-blue-100 text-blue-800'
                                        }`}>
                                            {user.role === 'PhotographyCompany' ? 'Photography Company' : 'Agent'}
                                        </span>
                                    </td>
                                    <td className="px-4 py-3 text-gray-600">
                                        {new Date(user.createdAt).toLocaleDateString('en-AU')}
                                    </td>
                                </tr>
                            ))
                        )}
                    </tbody>
                </table>
            </div>

            {result && (
                <div className="flex items-center justify-between mt-4 text-sm">
                    <span className="text-gray-600">
                        Page {result.pageNumber} of {result.totalPages} · {result.totalCount} total
                    </span>
                    <div className="flex gap-2">
                        <button
                            onClick={() => setPageNumber((prev) => prev - 1)}
                            disabled={!result.hasPreviousPage}
                            className="px-3 py-1.5 rounded border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            Previous
                        </button>
                        <button
                            onClick={() => setPageNumber((prev) => prev + 1)}
                            disabled={!result.hasNextPage}
                            className="px-3 py-1.5 rounded border border-gray-300 bg-white hover:bg-gray-100 disabled:opacity-50 disabled:cursor-not-allowed"
                        >
                            Next
                        </button>
                    </div>
                </div>
            )}
        </div>
    );
};

export default UserManagementPage;