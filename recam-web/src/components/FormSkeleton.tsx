import Skeleton from "./Skeleton";

interface FormSkeletonProps {
    fields?: number;
}

const FormSkeleton = ({ fields = 6 }: FormSkeletonProps) => {
    return (
        <div className="max-w-2xl">
            <Skeleton className="h-8 w-56 mb-4" />
            <div className="bg-white rounded-lg shadow p-6 flex flex-col gap-4">
                {Array.from({ length: fields }).map((_, index) => (
                    <div key={index} className="flex flex-col gap-1">
                        <Skeleton className="h-4 w-24" />
                        <Skeleton className="h-10 w-full" />
                    </div>
                ))}
                <Skeleton className="h-10 w-32 self-end" />
            </div>
        </div>
    );
};

export default FormSkeleton;