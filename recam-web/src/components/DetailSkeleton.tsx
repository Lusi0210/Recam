import Skeleton from "./Skeleton";

const DetailSkeleton = () => {
    return (
        <div className="max-w-4xl flex flex-col gap-6">
            <Skeleton className="h-4 w-32" />

            <div className="flex flex-col gap-2">
                <Skeleton className="h-8 w-2/3" />
                <Skeleton className="h-4 w-1/2" />
            </div>

            <div className="bg-white rounded-lg shadow p-6 flex flex-col gap-4">
                <Skeleton className="h-9 w-48" />
                <div className="grid grid-cols-2 sm:grid-cols-4 gap-4">
                    {Array.from({ length: 4 }).map((_, index) => (
                        <Skeleton key={index} className="h-16" />
                    ))}
                </div>
            </div>

            <div className="bg-white rounded-lg shadow p-6">
                <div className="grid grid-cols-2 sm:grid-cols-3 gap-4">
                    {Array.from({ length: 3 }).map((_, index) => (
                        <Skeleton key={index} className="h-40" />
                    ))}
                </div>
            </div>
        </div>
    );
};

export default DetailSkeleton;