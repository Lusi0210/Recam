import {  useEffect, useState, type SyntheticEvent } from "react";
import type { ListcaseStatus, ListingCase, PropertyType, SaleCategory, SortDirection, SortField } from "../types/listingCase";
import { getListingCases } from "../api/listingCaseApi";
import type { ApiResponse, PagedResult } from "../types/api";
import { LISTCASE_STATUS_LABELS, LISTCASE_STATUS_STYLES, PROPERTY_TYPE_LABELS, SALE_CATEGORY_LABELS } from "../constants/listingCase";
import axios from "axios";
import { Link } from "react-router-dom";
import { useAuth } from "../hooks/useAuth";

const PAGE_SIZE: number = 10;

interface AppliedFilters{
    search:string;
    city:string;
    minPrice:string;
    maxPrice:string;
}

const EMPTY_FILTERS:AppliedFilters = {search:'',city:'',minPrice:'',maxPrice:''};

export default function ListingCasesPage() {
    const [pageNumber,setPageNumber] = useState<number>(1);
    const [result, setResult] = useState<PagedResult<ListingCase> | null>(null);
    const [searchInput, setSearchInput] = useState<string>('');
    const [cityInput, setCityInput] = useState<string>('');
    const [minPriceInput, setMinPriceInput] = useState<string>('');
    const [maxPriceInput, setMaxPriceInput] = useState<string>('');
    const [applied, setApplied] = useState<AppliedFilters>(EMPTY_FILTERS);
    const [status, setStatus] = useState<string>('');
    const [propertyType, setPropertyType] = useState<string>('');
    const [saleCategory, setSaleCategory] = useState<string>('');
    const [sortBy, setSortBy] = useState<SortField | null>(null);
    const [sortDirection, setSortDirection] = useState<SortDirection>('asc');
    const [isLoading, setIsLoading] = useState<boolean>(true);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const { user } = useAuth();
    const canCreate: boolean = user?.role === 'PhotographyCompany';

    useEffect(()=>{
        const fetchData = async (): Promise<void> => {
            setIsLoading(true);
            setErrorMessage('');
            try {
                const data: PagedResult<ListingCase> = await getListingCases({ 
                    pageNumber, 
                    pageSize: PAGE_SIZE,
                    search: applied.search || undefined,
                    city: applied.city || undefined,
                    minPrice: applied.minPrice ? Number(applied.minPrice) : undefined,
                    maxPrice: applied.maxPrice ? Number(applied.maxPrice) : undefined,
                    status: status ? (Number(status) as ListcaseStatus) : undefined,
                    propertyType: propertyType ? (Number(propertyType) as PropertyType) : undefined,
                    saleCategory: saleCategory ? (Number(saleCategory) as SaleCategory) : undefined,
                    sortBy: sortBy ?? undefined,
                    sortDirection: sortBy ? sortDirection : undefined,
                });
                setResult(data);
            } catch (error: unknown) {
                if (axios.isAxiosError<ApiResponse<null>>(error)) {
                    const errors: string[] | null | undefined = error.response?.data?.errors;
                    if (errors && errors.length > 0) {
                        setErrorMessage(errors.join(', '));
                    } else {
                        setErrorMessage(error.response?.data?.message ?? 'Failed to load listing cases');
                    }
                } else {
                    setErrorMessage('Something went wrong');
                }

            } finally {
                setIsLoading(false);    
            }
            
        };
        fetchData();
    },[pageNumber, applied, status, propertyType, saleCategory, sortBy, sortDirection])

    const listingCases: ListingCase[] = result?.items ?? [];

    const handleSearch = (event: SyntheticEvent<HTMLFormElement>): void => {
        event.preventDefault();
        setApplied({
            search: searchInput,
            city: cityInput,
            minPrice: minPriceInput,
            maxPrice: maxPriceInput,
        });
        setPageNumber(1);
    };

    const handleReset = (): void => {
        setSearchInput('');
        setCityInput('');
        setMinPriceInput('');
        setMaxPriceInput('');
        setStatus('');
        setPropertyType('');
        setSaleCategory('');
        setApplied(EMPTY_FILTERS);
        setPageNumber(1);
        setSortBy(null);
        setSortDirection('asc');
    };

    const handleSort = (field: SortField): void => {
        if (sortBy === field) {
            setSortDirection((prev) => (prev === 'asc' ? 'desc' : 'asc'));
        } else {
            setSortBy(field);
            setSortDirection('asc');
        }
        setPageNumber(1);
    };

    const getSortIcon = (field: SortField): string => {
        if (sortBy !== field) return '↕';
        return sortDirection === 'asc' ? '↑' : '↓';
    };
    return (
    <div>
        <div className="flex items-center justify-between mb-4">
            <h1 className="text-2xl font-bold">Listing Cases</h1>
            {canCreate && (
                <Link
                    to="/listing-cases/new"
                    className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700"
                >
                    + New Listing Case
                </Link>
            )}
        </div>

        <form onSubmit={handleSearch} className="flex flex-wrap gap-2 mb-4">
            <input
                type="text"
                value={searchInput}
                onChange={(e) => setSearchInput(e.target.value)}
                placeholder="Search by title or address"
                className="flex-1 border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
                type="text"
                value={cityInput}
                onChange={(e) => setCityInput(e.target.value)}
                placeholder="City"
                className="w-32 border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
                type="number"
                value={minPriceInput}
                onChange={(e) => setMinPriceInput(e.target.value)}
                placeholder="Min price"
                className="w-32 border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <input
                type="number"
                value={maxPriceInput}
                onChange={(e) => setMaxPriceInput(e.target.value)}
                placeholder="Max price"
                className="w-32 border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500"
            />
            <select
                value={status}
                onChange={(e) => { setStatus(e.target.value); setPageNumber(1); }}
                className="border border-gray-300 rounded px-3 py-2 text-sm bg-white"
            >
                <option value="">All statuses</option>
                {Object.entries(LISTCASE_STATUS_LABELS).map(([value, label]) => (
                    <option key={value} value={value}>{label}</option>
                ))}
            </select>
            <select
                value={propertyType}
                onChange={(e) => { setPropertyType(e.target.value); setPageNumber(1); }}
                className="border border-gray-300 rounded px-3 py-2 text-sm bg-white"
            >
                <option value="">All types</option>
                {Object.entries(PROPERTY_TYPE_LABELS).map(([value, label]) => (
                    <option key={value} value={value}>{label}</option>
                ))}
            </select>
            <select
                value={saleCategory}
                onChange={(e) => { setSaleCategory(e.target.value); setPageNumber(1); }}
                className="border border-gray-300 rounded px-3 py-2 text-sm bg-white"
            >
                <option value="">All categories</option>
                {Object.entries(SALE_CATEGORY_LABELS).map(([value, label]) => (
                    <option key={value} value={value}>{label}</option>
                ))}
            </select>
            <button
                type="submit"
                className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700"
            >
                Search
            </button>
            <button
                type="button"
                onClick={handleReset}
                className="border border-gray-300 rounded px-4 py-2 text-sm hover:bg-gray-100"
            >
                Reset
            </button>
        </form>
        {errorMessage && (
            <p className="mb-4 text-sm text-red-600 bg-red-50 p-3 rounded">{errorMessage}</p>
        )}
        <div className="bg-white rounded-lg shadow overflow-x-auto">
            <table className="min-w-full text-sm">
                <thead className="bg-gray-50 text-left text-gray-600">
                    <tr>
                        <th className="px-4 py-3 font-medium">
                            <button type="button" onClick={() => handleSort('title')} className="flex items-center gap-1 hover:text-gray-900">
                                Title <span className="text-xs">{getSortIcon('title')}</span>
                            </button>
                        </th>
                        <th className="px-4 py-3 font-medium">
                            <button type="button" onClick={() => handleSort('city')} className="flex items-center gap-1 hover:text-gray-900">
                                Address <span className="text-xs">{getSortIcon('city')}</span>
                            </button>
                        </th>
                        <th className="px-4 py-3 font-medium">Type</th>
                        <th className="px-4 py-3 font-medium">Category</th>
                        <th className="px-4 py-3 font-medium text-right">
                            <button type="button" onClick={() => handleSort('price')} className="flex items-center gap-1 ml-auto hover:text-gray-900">
                                Price <span className="text-xs">{getSortIcon('price')}</span>
                            </button>
                        </th>
                        <th className="px-4 py-3 font-medium">
                            <button type="button" onClick={() => handleSort('status')} className="flex items-center gap-1 hover:text-gray-900">
                                Status <span className="text-xs">{getSortIcon('status')}</span>
                            </button>
                        </th>
                    </tr>
                </thead>
                <tbody className="divide-y divide-gray-100">
                    {isLoading ? (
                        <tr>
                            <td colSpan={6} className="px-4 py-8 text-center text-gray-500">Loading...</td>
                        </tr>
                    ) : listingCases.length === 0 ? (
                        <tr>
                            <td colSpan={6} className="px-4 py-8 text-center text-gray-500">No listing cases found</td>
                        </tr>
                    ) : (
                        listingCases.map((listingCase) => (
                            <tr key={listingCase.id} className="hover:bg-gray-50">
                            <td className="px-4 py-3 font-medium text-gray-900">
                                <Link to={`/listing-cases/${listingCase.id}`} className="hover:text-blue-600 hover:underline">
                                    {listingCase.title}
                                </Link>
                            </td>
                            <td className="px-4 py-3 text-gray-600">
                                {listingCase.street}, {listingCase.city} {listingCase.state}
                            </td>
                            <td className="px-4 py-3">{PROPERTY_TYPE_LABELS[listingCase.propertyType]}</td>
                            <td className="px-4 py-3">{SALE_CATEGORY_LABELS[listingCase.saleCategory]}</td>
                            <td className="px-4 py-3 text-right">
                                {listingCase.price.toLocaleString('en-AU', {
                                    style: 'currency',
                                    currency: 'AUD',
                                    maximumFractionDigits: 0,
                                })}
                            </td>
                            <td className="px-4 py-3">
                                <span className={`px-2 py-1 rounded-full text-xs font-medium ${LISTCASE_STATUS_STYLES[listingCase.listcaseStatus]}`}>
                                    {LISTCASE_STATUS_LABELS[listingCase.listcaseStatus]}
                                </span>
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
}