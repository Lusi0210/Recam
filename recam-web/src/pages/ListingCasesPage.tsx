import { useAuth } from "../hooks/useAuth";

export default function ListingCasesPage() {
  const {user} = useAuth();
  return <>
        <h1 className="text-3xl font-bold p-8">Listing Cases</h1>
        <p>{user?.email} - {user?.role}</p>;
    </>
}