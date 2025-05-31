"use client";
import {homeColumns} from '@/components/home/home-columns';
import {DataTable} from '@/components/data-table/data-table';
import {Tray} from '@prisma/client';
import {useEffect, useState} from 'react';
import {HomeDataTableToolbar} from '@/components/home/home-data-table-toolbar';

async function getData() {
    try {
        const response = await fetch('http://localhost:3000/api/tray/in-used');
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

export default function Home() {
    const [data, setData] = useState<Tray[]>([]);

    useEffect(() => {
        const fetchData = async () => {
            const traysData = await getData();
            setData(traysData);
        };

        fetchData();
        const interval = setInterval(fetchData, 1000); // Refresh every 1 seconds

        return () => clearInterval(interval); // Cleanup interval on component unmount
    }, []);

    return (
        <>
            <div className="flex justify-center">
                <div className="container mx-auto py-10">
                    <DataTable columns={homeColumns} data={data} pageSizeOptions={[5, 10, 20]} defaultPageSize={5}
                               toolbar={HomeDataTableToolbar}/>
                </div>
            </div>
        </>
    );
}