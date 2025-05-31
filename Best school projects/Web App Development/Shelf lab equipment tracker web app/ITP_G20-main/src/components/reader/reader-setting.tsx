//src/components/reader/reader-settings.tsx

"use client";

import {Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList} from "@/components/ui/command";
import {Popover, PopoverContent, PopoverTrigger} from "@/components/ui/popover";
import {cn} from "@/lib/utils";
import {Check, ChevronsUpDown} from "lucide-react";
import {Button} from "@/components/ui/button";
import {useEffect, useReducer, useState} from "react";
import {ReaderWithAntennas} from "@/data/local/readerRepo";
import {Form, FormControl, FormField, FormItem, FormLabel, FormMessage} from "@/components/ui/form";
import {editAntennaFunctionSchena} from "@/schema/custom";
import {z} from "zod";
import {zodResolver} from "@hookform/resolvers/zod";
import {useForm} from "react-hook-form";
import {Select, SelectContent, SelectItem, SelectTrigger, SelectValue} from "@/components/ui/select";
import {Function} from "@prisma/client";
import {Label} from "@/components/ui/label";
import {deleteReader, updateAllAntennaFunction} from "@/domain/readerControl";
import {toast} from "@/components/ui/use-toast";

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/reader');
        if (!response.ok) throw new Error('Network response was not ok');
        return await response.json();
    } catch (error) {
        return [];
    }
}

interface AntennaFunctionFormProps {
    selectedReader: ReaderWithAntennas | undefined;
}

enum AntennaFunction {
    SELECT_READER
}

interface AntennaFunctionAction {
    type: AntennaFunction;
    payload: any;
}

type EditAntennaFunction = z.infer<typeof editAntennaFunctionSchena>;

export function ReaderSetting() {
    const [open, setOpen] = useState(false);
    const [value, setValue] = useState("");
    const [readers, setReaders] = useState<ReaderWithAntennas[]>([]);
    const [state, dispatch] = useReducer(reducer, {
        selectedReader: undefined,
    });

    const form = useForm<EditAntennaFunction>({
        resolver: zodResolver(editAntennaFunctionSchena)
    });

    function reducer(state: AntennaFunctionFormProps, action: AntennaFunctionAction): AntennaFunctionFormProps {
        switch (action.type) {
            case AntennaFunction.SELECT_READER:
                const reader = readers.find((reader) => reader.ip === action.payload);
                return {
                    ...state,
                    selectedReader: reader,
                };
            default:
                return state;
        }
    }

    useEffect(() => {
        const fetchData = async () => {
            const readerData = await getData();
            setReaders(readerData);
        };
        fetchData();
        const interval = setInterval(fetchData, 1000);
        return () => clearInterval(interval);
    }, []);

    useEffect(() => {
        dispatch({
            type: AntennaFunction.SELECT_READER,
            payload: value
        });
    }, [value]);

    useEffect(() => {
        form.reset({
            antenna1: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 1)?.function || undefined,
            antenna2: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 2)?.function || undefined,
            antenna3: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 3)?.function || undefined,
            antenna4: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 4)?.function || undefined,
            antenna5: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 5)?.function || undefined,
            antenna6: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 6)?.function || undefined,
            antenna7: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 7)?.function || undefined,
            antenna8: state.selectedReader?.antennas.find((antenna) => antenna.antennaPort === 8)?.function || undefined,
        });
    }, [state.selectedReader, form]);

    async function onSubmit(data: EditAntennaFunction) {
        if (!state.selectedReader) {
            toast({
                title: "No reader selected",
            });
            return;
        }

        const error = await updateAllAntennaFunction(state.selectedReader.ip, data);
        if (error) {
            toast({
                variant: "destructive",
                title: error.error,
            });
        } else {
            toast({
                title: "Antenna functions updated",
            });
        }
    }

    async function handleDelete() {
        if (!state.selectedReader) {
            toast({
                title: "No reader selected",
            });
            return;
        }

        const error = await deleteReader(state.selectedReader.ip);

        if (error) {
            toast({
                variant: "destructive",
                title: error.error,
            });
        } else {
            toast({
                title: "Reader deleted",
            });
        }

        setValue("");
    }

    return (
        <div className="space-y-8 flex flex-col">
            <Label>Select a reader to configure</Label>
            <div className="flex space-x-4">
                <Popover open={open} onOpenChange={setOpen}>
                    <PopoverTrigger asChild>
                        <Button
                            variant="outline"
                            role="combobox"
                            aria-expanded={open}
                            className="w-[200px] justify-between"
                        >
                            {value
                                ? readers.find((reader) => reader.ip === value)?.ip
                                : "Select reader..."}
                            <ChevronsUpDown className="ml-2 h-4 w-4 shrink-0 opacity-50"/>
                        </Button>
                    </PopoverTrigger>
                    <PopoverContent className="w-[200px] p-0">
                        <Command>
                            <CommandInput placeholder="Search reader..."/>
                            <CommandList>
                                <CommandEmpty>No readers found</CommandEmpty>
                                <CommandGroup>
                                    {readers.map((reader) => (
                                        <CommandItem
                                            key={reader.ip}
                                            value={reader.ip}
                                            onSelect={(currentValue) => {
                                                setValue(currentValue === value ? "" : currentValue);
                                                setOpen(false);
                                            }}
                                        >
                                            <Check
                                                className={cn(
                                                    "mr-2 h-4 w-4",
                                                    value === reader.ip ? "opacity-100" : "opacity-0"
                                                )}
                                            />
                                            {reader.ip}
                                        </CommandItem>
                                    ))}
                                </CommandGroup>
                            </CommandList>
                        </Command>
                    </PopoverContent>
                </Popover>
                <Button disabled={!state.selectedReader} variant="destructive" onClick={handleDelete}>Delete
                    Reader</Button>
            </div>
            <Form {...form}>
                <form onSubmit={form.handleSubmit(onSubmit)} className="space-y-8">
                    <div className="grid grid-cols-4 gap-4">
                        {Array.from({length: 8}).map((_, index) => (
                            <FormField
                                key={index}
                                control={form.control}
                                name={`antenna${index + 1}` as keyof EditAntennaFunction}
                                render={({field}) => (
                                    <FormItem>
                                        <FormLabel>Antenna {index + 1}</FormLabel>
                                        <FormControl>
                                            <Select
                                                disabled={!state.selectedReader}
                                                value={field.value}
                                                onValueChange={(value) => field.onChange(value)}
                                            >
                                                <SelectTrigger>
                                                    <SelectValue placeholder="Select function"/>
                                                </SelectTrigger>
                                                <SelectContent>
                                                    {Object.values(Function).map((value) => (
                                                        <SelectItem key={value} value={value}>
                                                            {value}
                                                        </SelectItem>
                                                    ))}
                                                </SelectContent>
                                            </Select>
                                        </FormControl>
                                        <FormMessage/>
                                    </FormItem>
                                )}
                            />
                        ))}
                    </div>
                    <Button disabled={!state.selectedReader} type="submit">Set Antenna Functions</Button>
                </form>
            </Form>
        </div>
    );
}