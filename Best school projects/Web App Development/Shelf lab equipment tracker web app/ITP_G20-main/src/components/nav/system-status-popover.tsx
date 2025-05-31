"use client";

import {Button} from "@/components/ui/button";
import {useEffect, useState} from "react";
import {SystemStatus} from "@prisma/client";
import {BounceLoader} from "react-spinners";
import {startInventory, stopInventory} from "@/domain/rfidControl";
import {toast} from "@/components/ui/use-toast";
import {User} from "next-auth";

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/system-status');
        if (!response.ok) throw new Error('Network response was not ok');
        return await response.json();
    } catch (error) {
        console.error('Fetch operation error:', error);
        return [];
    }
}

interface SystemStatusPopoverProps {
    user?: User
}

export function SystemStatusPopover(
    {user}: SystemStatusPopoverProps
) {
    const [data, setData] = useState<SystemStatus>();
    const [status, setStatus] = useState<boolean>(false);

    useEffect(() => {
        const fetchData = async () => {
            const statusData = await getData();
            setData(statusData);
            setStatus(statusData?.systemStatus);
        };
        fetchData();
        const interval = setInterval(fetchData, 1000);
        return () => clearInterval(interval);
    }, []);

    async function customSubmit() {
        try {
            const newStatus = !status;
            console.log(newStatus)
            if (newStatus) {
                // Start inventory
                const response = await startInventory();
                if (response?.error) {
                    toast({
                        variant: "destructive",
                        title: response.error
                    })
                    return;
                }
            }
            else {
                // Stop inventory
                await stopInventory();
            }
            setStatus(newStatus);
            console.log(newStatus)
        } catch (error) {
            console.error('Form submission error:', error);
        }
    }

    return (
                <Button variant="outline" className="flex items-center space-x-2" onClick={customSubmit} disabled={!user}>
                    <BounceLoader size={20} color="#36d7b7" loading={status}/>
                    <div>
                        Inventory Scan is {status ? "Running" : "Stopped"} since {data?.systemTime ? new Date(data.systemTime).toLocaleDateString() : "N/A"}
                    </div>
                </Button>
    );
}