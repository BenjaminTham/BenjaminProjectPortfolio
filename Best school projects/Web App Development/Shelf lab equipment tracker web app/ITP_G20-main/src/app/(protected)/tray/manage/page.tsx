"use client";

import {useEffect, useState} from 'react';
import {Tray} from '@prisma/client';
import {DataTable} from '@/components/data-table/data-table';
import {trayColumns} from '@/components/tray/tray-columns';
import {DataTableToolbar} from '@/components/tray/tray-toolbar';

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/tray/all', {
            method: 'GET',
            headers: {
                'Content-Type': 'application/json',
            },
        });
        if (!response.ok) {
            throw new Error('Network response was not ok');
        }
        const trays = await response.json();
        return trays;
    } catch (error) {
        console.error('There was a problem with the fetch operation:', error);
        return [];
    }
}

export default function ManageTrays() {
    const [data, setData] = useState<Tray[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            const traysData = await getData();
            setData(traysData.map((tray: Tray) => ({
                ...tray,
                rackId: tray.rackId ? tray.rackId : 'N/A',
            })));
        };

        fetchData();
        const interval = setInterval(fetchData, 1000); // Refresh every 1 seconds

        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    return (
        <>
            <div className="flex justify-center">
                <div className="container mx-auto py-10">
                    <DataTable
                        columns={trayColumns}
                        data={data}
                        pageSizeOptions={[5, 10, 20]}
                        defaultPageSize={5}
                        toolbar={DataTableToolbar}
                    />
                </div>
            </div>
        </>
    );
}
