'use client'

import {useEffect, useState} from 'react';
import {DataTable} from '@/components/data-table/data-table';
import {rackColumns} from '@/components/rack/rack-columns';
import {DataTableToolbar} from '@/components/rack/rack-toolbar'
import {Rack} from '@prisma/client';


async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/rack');
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

export default function RackPage() {
    const [rackData, setRackData] = useState<Rack[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            const racks = await getData();
            setRackData(racks);
        };

        fetchData();
        const interval = setInterval(fetchData, 1000); // Refresh every 1 seconds

        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    return (
        <div className="flex justify-center">
            <div className="container mx-auto py-10">
                <DataTable
                    columns={rackColumns}
                    data={rackData}
                    pageSizeOptions={[5, 10, 20]}
                    defaultPageSize={5}
                    toolbar={DataTableToolbar}
                />
            </div>
        </div>
    )
}