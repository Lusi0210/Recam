import { Link } from "react-router-dom";

const UnauthorizedPage = () => {
    return (
        <div className="min-h-screen flex flex-col items-center justify-center gap-4 bg-gray-100">
            <h1 className="text-2xl font-bold">You don't have permission to view this page</h1>
            <Link to="/" className="text-blue-600 hover:underline">Back to home</Link>
        </div>
    );
};

export default UnauthorizedPage;