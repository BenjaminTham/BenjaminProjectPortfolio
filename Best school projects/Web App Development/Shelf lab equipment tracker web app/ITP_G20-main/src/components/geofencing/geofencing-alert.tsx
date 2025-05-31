"use client";

import {
    AlertDialog,
    AlertDialogAction,
    AlertDialogContent,
    AlertDialogDescription,
    AlertDialogFooter,
    AlertDialogHeader,
    AlertDialogTitle,
} from "@/components/ui/alert-dialog";
import {useEffect, useState} from "react";
import {Tray} from "@prisma/client";
import {setTrayOutOfBound} from "@/domain/trayControl";

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/tray/out-of-bound');
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        return await response.json();
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        return [];
    }
}

export function GeofencingAlert() {
    const [isOpen, setIsOpen] = useState(false);
    const [data, setData] = useState<Tray[]>([]);
    useEffect(() => {
        const fetchData = async () => {
            const traysData = await getData();
            setData(traysData);
            if (traysData.length > 0) {
                setIsOpen(true);
            }
        };

        fetchData();
        const interval = setInterval(fetchData, 1000); // Refresh every 1 seconds

        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    const handleContinue = async () => {
        await Promise.all(data.map(tray => setTrayOutOfBound(tray.epc, false)));
    }

    return (
        <AlertDialog open={isOpen} onOpenChange={setIsOpen}>
            <AlertDialogContent>
                <AlertDialogHeader>
                    <AlertDialogTitle>Warning: Trays is out of bounds</AlertDialogTitle>
                    <AlertDialogDescription>
                        The following trays are out of bounds:
                        <ul className="my-6 ml-6 list-disc [&>li]:mt-2">
                            {data.map(tray => (
                                <li key={tray.id}>
                                    {tray.id} at {
                                    tray.statusChg
                                        ? new Date(tray.statusChg).toLocaleString('en-US', {timeZone: 'Asia/Singapore'})
                                        : 'Unknown time'
                                }
                                </li>
                            ))}
                        </ul>
                        <p>Press &quot;Continue&quot; to acknowledge this alert. It will set the trays back to inbounds.</p>
                    </AlertDialogDescription>
                </AlertDialogHeader>
                <AlertDialogFooter>
                    <AlertDialogAction onClick={handleContinue}>Continue</AlertDialogAction>
                </AlertDialogFooter>
            </AlertDialogContent>
        </AlertDialog>
    );
}