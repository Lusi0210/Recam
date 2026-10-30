import { useEffect, useState, type SyntheticEvent } from "react";
import { getListingCaseById, updateListingCase } from "../api/listingCaseApi";
import { useNavigate, useParams } from "react-router-dom";
import type { ListingCaseDetailsResponse, SaleCategory} from "../types/listingCase";
import { SALE_CATEGORY_LABELS } from "../constants/listingCase";
import axios from "axios";
import type { ApiResponse } from "../types/api";

const inputClass: string =
    "w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";

const EditListingCasePage = () => {
    const {id} = useParams<{id : string}>()
    const [title,setTitle] = useState<string>('');
    const [description, setDescription] = useState<string>('');
    const [price,setPrice] = useState<string>('');
    const [bedrooms,setBedrooms] = useState<string>('');
    const [bathrooms,setBathrooms] = useState<string>('');
    const [garages,setGarages] = useState<string>('');
    const [floorArea,setFloorArea] = useState<string>('');
    const [saleCategory, setSaleCategory] = useState<string>('');
    const [isLoading,setIsLoading] = useState<boolean>(true); 
    const [errorMessage,setErrorMessage] = useState<string>('');
    const [isSaving,setIsSaving] = useState<boolean>(false);
    const navigate = useNavigate();
    const [loadError, setLoadError] = useState<string>('');

    useEffect(()=>{
        const fetchData =  async () => {
            setIsLoading(true);
            setLoadError(''); 
            try {
                const response:ListingCaseDetailsResponse = await getListingCaseById(Number(id));
                setTitle(response.title);
                setDescription(response.description ?? '');                       
                setPrice(String(response.price));
                setBedrooms(String(response.bedrooms));
                setBathrooms(String(response.bathrooms));
                setGarages(String(response.garages));
                setFloorArea(String(response.floorArea));
                setSaleCategory(String(response.saleCategory)); 
            } catch (error:unknown) {
                if (axios.isAxiosError<ApiResponse<null>>(error)) {
                    const errors: string[] | null | undefined = error.response?.data?.errors;
                    if (errors && errors.length > 0) {
                        setLoadError(errors.join(', ')); 
                    } else {
                        setLoadError(error.response?.data?.message ?? 'Failed to load listing case');
                    }
                } else {
                    setLoadError('Something went wrong');
                }
            } finally {
                setIsLoading(false);
            }
        }

        fetchData();
    },[id])

    const handleSubmit = async (event:SyntheticEvent<HTMLFormElement>) => {
        event.preventDefault()
        setIsSaving(true);
        setErrorMessage('')
        try {
            await updateListingCase({
            title,
            description,
            price:Number(price),
            bedrooms:Number(bedrooms),
            bathrooms:Number(bathrooms),
            garages:Number(garages),
            floorArea:Number(floorArea),
            saleCategory:Number(saleCategory) as SaleCategory},
            Number(id));
            navigate(`/listing-cases/${id}`);
        } catch (error:unknown) {
            if (axios.isAxiosError<ApiResponse<null>>(error)) {
                const errors: string[] | null | undefined = error.response?.data?.errors;
                if (errors && errors.length > 0) {
                    setErrorMessage(errors.join(', '));
                } else {
                    setErrorMessage(error.response?.data?.message ?? 'Failed to update listing case');
                }
            } else {
                setErrorMessage('Something went wrong');
            }
        } finally {
            setIsSaving(false);
        }
        
    }

    if (isLoading) {
        return <p className="text-gray-500">Loading...</p>;
    }

    if (loadError) {
        return <p className="text-sm text-red-600 bg-red-50 p-3 rounded">{loadError}</p>;
    }

    return (
        <div className="max-w-2xl">
            <h1 className="text-2xl font-bold mb-4">Edit Listing Case</h1>

            <form className="bg-white rounded-lg shadow p-6 flex flex-col gap-4" onSubmit={handleSubmit}>
                {errorMessage && (
                    <p className="text-sm text-red-600 bg-red-50 p-2 rounded">{errorMessage}</p>
                )}
                <label className="flex flex-col gap-1 text-sm font-medium">
                    Title
                    <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} className={inputClass} required/>
                </label>

                <label className="flex flex-col gap-1 text-sm font-medium">
                    Description
                    <textarea value={description} onChange={(e) => setDescription(e.target.value)} rows={3} className={inputClass}/>
                </label>

                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Price (AUD)
                        <input type="number" value={price} onChange={(e) => setPrice(e.target.value)} className={inputClass} required  min="0" step="any"/>
                    </label>
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Sale category
                        <select value={saleCategory} onChange={(e) => setSaleCategory(e.target.value)} className={`${inputClass} bg-white`} required>
                            <option value="">Select category</option>
                            {Object.entries(SALE_CATEGORY_LABELS).map(([value, label]) => (
                                <option key={value} value={value}>{label}</option>
                            ))}
                        </select>
                    </label>
                </div>

                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Bedrooms
                        <input type="number" value={bedrooms} onChange={(e) => setBedrooms(e.target.value)} className={inputClass}  required  min="0" step="1"/>
                    </label>
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Bathrooms
                        <input type="number" value={bathrooms} onChange={(e) => setBathrooms(e.target.value)} className={inputClass}  required  min="0" step="1"/>
                    </label>
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Garages
                        <input type="number" value={garages} onChange={(e) => setGarages(e.target.value)} className={inputClass} required  min="0" step="1" />
                    </label>
                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Floor area (m²)
                        <input type="number" value={floorArea} onChange={(e) => setFloorArea(e.target.value)} className={inputClass} required  min="0" step="any" />
                    </label>
                </div>

                <div className="flex justify-end gap-2">
                    <button type="button" onClick={() => navigate(`/listing-cases/${id}`)} className="border border-gray-300 rounded px-4 py-2 text-sm hover:bg-gray-100">
                        Cancel
                    </button>
                    <button type="submit" disabled={isSaving} className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed">
                        {isSaving ? 'Saving...' : 'Save changes'}
                    </button>
                </div>
            </form>
        </div>
    );
}

export default EditListingCasePage;