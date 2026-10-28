import { useState, type SyntheticEvent } from "react";
import { PROPERTY_TYPE_LABELS, SALE_CATEGORY_LABELS } from "../constants/listingCase";
import type { CreateListingCaseRequest, PropertyType, SaleCategory } from "../types/listingCase";
import { createListingCase } from "../api/listingCaseApi";
import { useNavigate } from "react-router-dom";
import axios from "axios";
import type { ApiResponse } from "../types/api";


const inputClass: string =
    "w-full border border-gray-300 rounded px-3 py-2 text-sm focus:outline-none focus:ring-2 focus:ring-blue-500";

const CreateListingCasePage = () => {
    const [title,setTitle] = useState<string>('');
    const [description,setDescription] = useState<string>('');
    const [street,setStreet] = useState<string>('')
    const [city,setCity] = useState<string>('')
    const [state,setState] = useState<string>('')
    const [postcode,setPostcode] = useState<string>('')
    const [longitude,setLongitude] = useState<string>('')
    const [latitude,setLatitude] = useState<string>('')
    const [price,setPrice] = useState<string>('')
    const [bedrooms,setBedrooms] = useState<string>('')
    const [bathrooms,setBathrooms] = useState<string>('')
    const [garage,setGarage] = useState<string>('')
    const [floorArea,setFloorArea] = useState<string>('')
    const [propertyType,setPropertyType] = useState<string>('')
    const [saleCategory,setSaleCategory] = useState<string>('')
    const [isLoading,setIsLoading] = useState<boolean>(false);
    const [errorMessage,setErrorMessage] = useState<string>('');
    const navigate = useNavigate();

    const handleSubmit = async (event:SyntheticEvent<HTMLFormElement>) : Promise<void>=> {
        event.preventDefault();
        setErrorMessage('');
        setIsLoading(true);
        try {
            const request:CreateListingCaseRequest = {
            title,
            description,
            street,
            city,
            state,
            postcode,
            longitude: Number(longitude),
            latitude:Number(latitude),
            price:Number(price),
            bedrooms:Number(bedrooms),
            bathrooms:Number(bathrooms),
            garages:Number(garage),
            floorArea:Number(floorArea),
            propertyType:Number(propertyType) as PropertyType,
            saleCategory:Number(saleCategory) as SaleCategory
            };
            await createListingCase(request)
            navigate('/')
        } catch (error: unknown) {
            if (axios.isAxiosError<ApiResponse<null>>(error)) {
                const errors: string[] | null | undefined = error.response?.data?.errors;
                if (errors && errors.length > 0) {
                    setErrorMessage(errors.join(', '));
                } else {
                    setErrorMessage(error.response?.data?.message ?? 'Failed to create listing case');
                }
            } else {
                setErrorMessage('Something went wrong');
            }
        } finally {
                setIsLoading(false);    
        }
        
    }

    return (
            <div className="max-w-2xl">
                <h1 className="text-2xl font-bold mb-4">Create Listing Case</h1>

                <form onSubmit={handleSubmit} className="bg-white rounded-lg shadow p-6 flex flex-col gap-4">
                    {errorMessage && (
                        <p className="text-sm text-red-600 bg-red-50 p-2 rounded">{errorMessage}</p>
                    )}

                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Title
                        <input type="text" value={title} onChange={(e) => setTitle(e.target.value)} required className={inputClass} />
                    </label>

                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Description
                        <textarea value={description} onChange={(e) => setDescription(e.target.value)} rows={3} className={inputClass} />
                    </label>

                    <label className="flex flex-col gap-1 text-sm font-medium">
                        Street
                        <input type="text" value={street} onChange={(e) => setStreet(e.target.value)} required className={inputClass} />
                    </label>

                    <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            City
                            <input type="text" value={city} onChange={(e) => setCity(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            State
                            <input type="text" value={state} onChange={(e) => setState(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Postcode
                            <input type="text" value={postcode} onChange={(e) => setPostcode(e.target.value)} required className={inputClass} />
                        </label>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-3 gap-4">
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Price (AUD)
                            <input type="number" min="0" step="any" value={price} onChange={(e) => setPrice(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Latitude
                            <input type="number" step="any" value={latitude} onChange={(e) => setLatitude(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Longitude
                            <input type="number" step="any" value={longitude} onChange={(e) => setLongitude(e.target.value)} required className={inputClass} />
                        </label>
                    </div>

                    <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Bedrooms
                            <input type="number" min="0" step="1" value={bedrooms} onChange={(e) => setBedrooms(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Bathrooms
                            <input type="number" min="0" step="1" value={bathrooms} onChange={(e) => setBathrooms(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Garages
                            <input type="number" min="0" step="1" value={garage} onChange={(e) => setGarage(e.target.value)} required className={inputClass} />
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Floor area (m²)
                            <input type="number" min="0" step="any" value={floorArea} onChange={(e) => setFloorArea(e.target.value)} required className={inputClass} />
                        </label>
                    </div>

                    <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Property type
                            <select value={propertyType} onChange={(e) => setPropertyType(e.target.value)} required className={`${inputClass} bg-white`}>
                                <option value="">Select type</option>
                                {Object.entries(PROPERTY_TYPE_LABELS).map(([value, label]) => (
                                    <option key={value} value={value}>{label}</option>
                                ))}
                            </select>
                        </label>
                        <label className="flex flex-col gap-1 text-sm font-medium">
                            Sale category
                            <select value={saleCategory} onChange={(e) => setSaleCategory(e.target.value)} required className={`${inputClass} bg-white`}>
                                <option value="">Select category</option>
                                {Object.entries(SALE_CATEGORY_LABELS).map(([value, label]) => (
                                    <option key={value} value={value}>{label}</option>
                                ))}
                            </select>
                        </label>
                    </div>

                    <div className="flex justify-end gap-2">
                        <button type="button" onClick={() => navigate('/')} className="border border-gray-300 rounded px-4 py-2 text-sm hover:bg-gray-100">
                            Cancel
                        </button>
                        <button type="submit" disabled={isLoading} className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:bg-gray-400 disabled:cursor-not-allowed">
                            {isLoading ? 'Creating...' : 'Create'}
                        </button>
                    </div>
                </form>
            </div>
        );
}

export default CreateListingCasePage;