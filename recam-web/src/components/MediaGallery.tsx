import type { MediaAsset } from "../types/mediaAsset";

interface MediaGalleryProps{
    mediaAssets:MediaAsset[]
}

const MediaGallery = ({mediaAssets}:MediaGalleryProps) => {

    const photoAssets:MediaAsset[] = mediaAssets.filter((media) => media.mediaType === 1); 
    const videoAssets:MediaAsset[] = mediaAssets.filter((media) => media.mediaType === 2);
    const floorPlanAssets:MediaAsset[] = mediaAssets.filter((media)=> media.mediaType === 3);

    return <div className="flex flex-col gap-6">
                {photoAssets.length > 0 && (
                    <section>
                        <h3 className="text-sm font-semibold text-gray-700 mb-2">Photos</h3>
                        <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
                            {photoAssets.map((media) => (
                                    <div key={media.id} className="relative">
                                        <img
                                            src={media.mediaUrl}
                                            // alt={result.title}
                                            className="w-full h-40 object-cover rounded"
                                        />
                                        {media.isHero && (
                                            <span className="absolute top-2 left-2 bg-blue-600 text-white text-xs font-medium px-2 py-1 rounded">
                                                Cover
                                            </span>
                                        )}
                                    </div>
                                ))}
                        </div>
                    </section>
                )}
                
                {videoAssets.length > 0 && (
                    <section>
                        <h3>Videos</h3>
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            {videoAssets.map((media) => (
                                <video
                                    key={media.id}
                                    src={media.mediaUrl}
                                    controls
                                    className="w-full h-56 rounded bg-black"
                                />
                            ))}
                        </div>
                    </section>
                )}

                {floorPlanAssets.length > 0 && (
                    <section>
                        <h3 className="text-sm font-semibold text-gray-700 mb-2">Floor Plans</h3>
                        <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                            {floorPlanAssets.map((media) => (
                                <img
                                    key={media.id}
                                    src={media.mediaUrl}
                                    alt="Floor plan"
                                    className="w-full h-56 object-contain rounded bg-white border border-gray-200"
                                />
                            ))}
                        </div>
                    </section>
                )}
            </div>

} 
export default MediaGallery;