import Roles from "@/containers/Roles/Roles";
export default async function Page({ params }: { params: Promise<{ id: string }> }) {
  const { id } = await params;
  return <Roles mode="view" id={id} />;
}
