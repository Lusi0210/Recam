import { useState } from "react";
import axios from "axios";
import { uploadMedia } from "../api/mediaApi";
import { MEDIA_TYPE_LABELS } from "../constants/mediaAsset";
import type { MediaAsset, MediaType } from "../types/mediaAsset";
import type { ApiResponse } from "../types/api";

interface MediaUploaderProps {
    listingCaseId: number;
    onUploaded: (newMedia: MediaAsset[]) => void;
}

const MediaUploader = ({ listingCaseId, onUploaded }: MediaUploaderProps) => {
    const [files, setFiles] = useState<File[]>([]);
    const [mediaType, setMediaType] = useState<string>('');
    const [isUploading, setIsUploading] = useState<boolean>(false);
    const [errorMessage, setErrorMessage] = useState<string>('');
    const [isDragging, setIsDragging] = useState<boolean>(false);
    const [progress, setProgress] = useState<number>(0);
    const [successMessage, setSuccessMessage] = useState<string>('');

    const handleUpload = async (): Promise<void> => {
        if (files.length === 0 || !mediaType) {
            setErrorMessage('Please choose files and a media type');
            return;
        }
        setIsUploading(true);
        setProgress(0)
        setErrorMessage('');
        try {
            const uploaded: MediaAsset[] = await uploadMedia(listingCaseId, files, Number(mediaType) as MediaType,setProgress);
            onUploaded(uploaded);
            setFiles([]);
            {successMessage && (
                <p className="text-sm text-green-700 bg-green-50 p-2 rounded">{successMessage}</p>
            )}
        } catch (error: unknown) {
            if (axios.isAxiosError<ApiResponse<null>>(error)) {
                const errors: string[] | null | undefined = error.response?.data?.errors;
                setErrorMessage(
                    errors && errors.length > 0
                        ? errors.join(', ')
                        : error.response?.data?.message ?? 'Upload failed'
                );
            } else {
                setErrorMessage('Something went wrong');
            }
        } finally {
            setIsUploading(false);
        }
    };

    const handleRemove = (indexToRemove:number) => {
        setFiles((prev)=> prev.filter((_,index) => index!==indexToRemove));
    }

    const addFiles = (newFiles: File[]): void => {
        setFiles((prev) => [...prev, ...newFiles]);
        setSuccessMessage('');
    };

    return (
        <div className="flex flex-col gap-3">
            {errorMessage && (
                <p className="text-sm text-red-600 bg-red-50 p-2 rounded">{errorMessage}</p>
            )}

            <div className="flex flex-wrap items-center gap-2">
                <select
                    value={mediaType}
                    onChange={(e) => setMediaType(e.target.value)}
                    className="border border-gray-300 rounded px-3 py-2 text-sm bg-white"
                >
                    <option value="">Select media type</option>
                    {Object.entries(MEDIA_TYPE_LABELS).map(([value, label]) => (
                        <option key={value} value={value}>{label}</option>
                    ))}
                </select>

                <label onDragOver={(e) => {
                    e.preventDefault();
                    setIsDragging(true);
                    }}
                    onDragLeave={() => setIsDragging(false)}
                    onDrop={(e)=>{
                        e.preventDefault();
                        setIsDragging(false);
                        addFiles(Array.from(e.dataTransfer.files));
                       }}
                className={`flex flex-col items-center justify-center w-full h-32 border-2 border-dashed rounded cursor-pointer text-sm ${
                    isDragging ? 'border-blue-500 bg-blue-50 text-blue-600' : 'border-gray-300 text-gray-500 hover:bg-gray-50'
                }`}>
                    <span>Drag & drop files here, or click to browse</span>
                    <input
                        type="file"
                        multiple
                        accept="image/*,video/*"
                        onChange={(e) => {
                            addFiles(Array.from(e.target.files ?? []));
                            e.target.value='';
                        }}
                        className="hidden"
                    />
                </label>
                

                <button
                    type="button"
                    onClick={handleUpload}
                    disabled={isUploading}
                    className="bg-blue-600 text-white rounded px-4 py-2 text-sm font-medium hover:bg-blue-700 disabled:bg-gray-400"
                >
                    {isUploading ? 'Uploading...' : 'Upload'}
                </button>
                                {isUploading && (
                    <div className="w-full bg-gray-200 rounded h-2 overflow-hidden">
                        <div
                            className="bg-blue-600 h-2 transition-all"
                            style={{ width: `${progress}%` }}
                        />
                    </div>
                )}
            </div>

            {files.length > 0 && (
                <ul className="grid grid-cols-2 sm:grid-cols-4 gap-3">
                    {files.map((file,index) => (
                        <li key={`${file.name}-${index}`} className="relative border border-gray-200 rounded overflow-hidden bg-gray-50">
                            {file.type.startsWith('image/') ? (
                                <img
                                    src={URL.createObjectURL(file)}
                                    alt={file.name}
                                    className="w-full h-24 object-cover"
                                />
                            ) : (
                                <div className="w-full h-24 flex items-center justify-center text-xs text-gray-500">
                                    Video
                                </div>
                            )}
                            <p className="text-xs px-2 pt-1 truncate">{file.name}</p>
                            <p className="text-xs px-2 pb-1 text-gray-500">{(file.size / 1024 / 1024).toFixed(1)} MB</p>

                            <button
                                type="button"
                                onClick={() => handleRemove(index)}
                                className="absolute top-1 right-1 w-6 h-6 rounded-full bg-white/90 text-gray-600 hover:text-red-600 text-sm leading-none"
                                aria-label={`Remove ${file.name}`}
                            >
                                X
                            </button>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
};

export default MediaUploader;