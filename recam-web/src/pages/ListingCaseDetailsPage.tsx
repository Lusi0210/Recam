import { Link, useParams } from "react-router-dom";
import { getListingCaseById } from "../api/listingCaseApi";
import { useEffect, useState } from "react";
import type { ListingCaseDetailsResponse } from "../types/listingCase";
import axios from "axios";
import type { ApiResponse } from "../types/api";
import { LISTCASE_STATUS_LABELS, LISTCASE_STATUS_STYLES, PROPERTY_TYPE_LABELS, SALE_CATEGORY_LABELS } from "../constants/listingCase";
import { formatPrice } from "../utils/format";
import { useAuth } from "../hooks/useAuth";
import MediaUploader from "../components/MediaUploader";
import type { MediaAsset } from "../types/mediaAsset";
import MediaGallery from "../components/MediaGallery";

const ListingCaseDetailsPage = () => {
    const {id} = useParams<{id : string}>()
    const [isLoading,setIsLoading] = useState<boolean>(true);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [result,setResult] = useState<ListingCaseDetailsResponse | null>(null);
    const { user } = useAuth();
    const canEdit: boolean = user?.role === 'PhotographyCompany';
    useEffect(()=>{
        const fetchData = async () :Promise<void> => {
            setIsLoading(true);
            setErrorMessage('');
            try {
                const data:ListingCaseDetailsResponse = await getListingCaseById(Number(id));
                setResult(data);
            } catch (error:unknown) {
                if (axios.isAxiosError<ApiResponse<null>>(error)) {
                    const errors: string[] | null | undefined = error.response?.data?.errors;
                    if (errors && errors.length > 0) {
                        setErrorMessage(errors.join(', '));
                    } else {
                        setErrorMessage(error.response?.data?.message ?? 'Failed to load listing case');
                    }
                } else {
                    setErrorMessage('Something went wrong');
                }
            } finally {
                setIsLoading(false);
            }
        }

        fetchData();
    },[id])

    const handleMediaUploaded = (newMedia: MediaAsset[]): void => {
        setResult((prev) =>
            prev ? { ...prev, mediaAssets: [...prev.mediaAssets, ...newMedia] } : prev
        );
    };

    
    if (isLoading) {
        return <p className="text-gray-500">Loading...</p>;
    }

    if (errorMessage) {
        return <p className="text-sm text-red-600 bg-red-50 p-3 rounded">{errorMessage}</p>;
    }

    if (!result) {
        return null;
    }

    const stats: { label: string; value: string }[] = [
        { label: 'Bedrooms', value: String(result.bedrooms) },
        { label: 'Bathrooms', value: String(result.bathrooms) },
        { label: 'Garages', value: String(result.garages) },
        { label: 'Floor area', value: `${result.floorArea} m²` },
    ];

    return (
        <div className="max-w-4xl flex flex-col gap-6">
            <Link to="/" className="text-sm text-blue-600 hover:underline">
                ← Back to listing cases
            </Link>

            <div className="flex flex-wrap items-start justify-between gap-4">
                <div>
                    <h1 className="text-2xl font-bold">{result.title}</h1>
                    <p className="text-gray-600 mt-1">
                        {result.street}, {result.city} {result.state} {result.postcode}
                    </p>
                </div>
                <div className="flex flex-col items-end gap-2">
                    <span className={`px-3 py-1 rounded-full text-sm font-medium ${LISTCASE_STATUS_STYLES[result.listcaseStatus]}`}>
                        {LISTCASE_STATUS_LABELS[result.listcaseStatus]}
                    </span>
                    {canEdit && (
                        <Link
                            to={`/listing-cases/${result.id}/edit`}
                            className="text-sm border border-gray-300 rounded px-3 py-1.5 hover:bg-gray-100"
                        >
                            Edit
                        </Link>
                    )}
                </div>
            </div>

            <div className="bg-white rounded-lg shadow p-6">
                <p className="text-3xl font-bold text-gray-900">{formatPrice(result.price)}</p>
                <p className="text-sm text-gray-500 mt-1">
                    {PROPERTY_TYPE_LABELS[result.propertyType]} · {SALE_CATEGORY_LABELS[result.saleCategory]}
                </p>

                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4 mt-6">
                    {stats.map((stat) => (
                        <div key={stat.label} className="bg-gray-50 rounded p-3">
                            <p className="text-xs text-gray-500">{stat.label}</p>
                            <p className="text-lg font-semibold text-gray-900">{stat.value}</p>
                        </div>
                    ))}
                </div>
            </div>

            {result.description && (
                <div className="bg-white rounded-lg shadow p-6">
                    <h2 className="text-lg font-semibold mb-2">Description</h2>
                    <p className="text-gray-700 whitespace-pre-line">{result.description}</p>
                </div>
            )}
            <div className="bg-white rounded-lg shadow p-6">
                <h2 className="text-lg font-semibold mb-4">
                    Media ({result.mediaAssets.length})
                </h2>

                {canEdit && (
                    <div className="mb-4">
                        <MediaUploader listingCaseId={result.id} onUploaded={handleMediaUploaded} />
                    </div>
                )}

                {result.mediaAssets.length === 0 ? (
                    <p className="text-sm text-gray-500">No media uploaded yet.</p>
                ) : (
                    <MediaGallery mediaAssets={result.mediaAssets}/>
                )}
            </div>
            <div className="bg-white rounded-lg shadow p-6">
                <h2 className="text-lg font-semibold mb-4">
                    Agents ({result.agents.length})
                </h2>

                {result.agents.length === 0 ? (
                    <p className="text-sm text-gray-500">No agents assigned yet.</p>
                ) : (
                    <ul className="flex flex-col gap-3">
                        {result.agents.map((agent) => (
                            <li key={agent.id} className="flex items-center gap-3">
                                <div className="w-10 h-10 rounded-full bg-blue-100 text-blue-700 flex items-center justify-center text-sm font-semibold">
                                    {agent.agentFirstName[0]}{agent.agentLastName[0]}
                                </div>
                                <div>
                                    <p className="text-sm font-medium text-gray-900">
                                        {agent.agentFirstName} {agent.agentLastName}
                                    </p>
                                    <p className="text-xs text-gray-500">
                                        {agent.companyName} · {agent.email}
                                    </p>
                                </div>
                            </li>
                        ))}
                    </ul>
                )}
            </div>
        </div>
    );
}

export default ListingCaseDetailsPage;