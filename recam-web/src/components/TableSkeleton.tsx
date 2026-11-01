import Skeleton from "./Skeleton";

interface TableSkeletonProps {
    rows?: number;
    columns: number;
}

const TableSkeleton = ({ rows = 5, columns }: TableSkeletonProps) => {
    return (
        <>
            {Array.from({ length: rows }).map((_, rowIndex) => (
                <tr key={rowIndex}>
                    {Array.from({ length: columns }).map((_, colIndex) => (
                        <td key={colIndex} className="px-4 py-3">
                            <Skeleton className="h-4 w-full" />
                        </td>
                    ))}
                </tr>
            ))}
        </>
    );
};

export default TableSkeleton;