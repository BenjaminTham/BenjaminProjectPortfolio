import {Table} from "@tanstack/react-table";
import {Input} from "@/components/ui/input";

interface DataTableTextFilterProps<TData> {
    table: Table<TData>
    accessorKey: string
    placeholder?: string
}

export function DataTableTextFilter<TData>({
                                               table,
                                               accessorKey,
                                               placeholder,
                                           }: DataTableTextFilterProps<TData>) {
    return (
        <Input
            placeholder={placeholder}
            value={(table.getColumn(accessorKey)?.getFilterValue() as string) ?? ""}
            onChange={(event) =>
                table.getColumn(accessorKey)?.setFilterValue(event.target.value)
            }
            className="h-8 w-[150px] lg:w-[250px]"
        />
    )
}