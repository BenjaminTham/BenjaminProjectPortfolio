"use client"

import {Button, Form, Input, message, Popconfirm, Space, Table, Typography} from "antd";
import {useForm} from "antd/lib/form/Form";
import {commandDoor, deleteMantrap, getData, getMantraps} from "@/app/lib/api";
import {useEffect, useState} from "react";
import {GetMantraps} from "@/app/lib/types";
import {ExclamationCircleFilled} from "@ant-design/icons";
import {MantrapModal} from "@/app/(authenticated)/mantraps/MantrapModal";

const {Title} = Typography;


export default function Mantraps() {

    const [form] = useForm()
    const [mantraps, setMantraps] = useState<GetMantraps["data"]>([])
    const [displayMessage, contextHolder] = message.useMessage()
    const [loading, setLoading] = useState(false)
    const [selected, setSelected] = useState<GetMantraps["data"][number] | null>(null)
    const [open, setOpen] = useState(false)
    const [isAdmin, setIsAdmin] = useState(false)

    useEffect(() => {
        loadMantraps();
        const interval = setInterval(loadMantraps, 5000); // Fetch every 5 seconds
        return () => clearInterval(interval); // Cleanup interval on unmount
    }, []);

    useEffect(() => {
        getData("user").then(user => setIsAdmin(user.role === "admin"))
    }, [])


    const clickSearch = () => {
        form.validateFields().then(values => loadMantraps(values))
    }

    const clickReset = () => {
        form.resetFields()
        loadMantraps()
    }

    const columns = [
        {title: "ID", dataIndex: "id"},
        {title: "Location", dataIndex: "location"},
        {title: "Token", dataIndex: "token"},
        {title: "Entry Gate", dataIndex: "entry_gate_status"},
        {title: "Exit Gate", dataIndex: "exit_gate_status"},
        {
            title: "Control",
            dataIndex: "entry_gate_status",
            render: (status: string, _: GetMantraps["data"][number]) => <Button
                onClick={() => controlGate(_.id, status === "CLOSED" ? "open" : "close")}>{status === "CLOSED" ? "Unlock" : "Lock"}</Button>
        },
        {
            title: "Actions", dataIndex: "id", render: (id: number) =>
                <Space.Compact>
                    <Button type="primary" onClick={() => clickEdit(id)} disabled={!isAdmin}>Edit</Button>
                    <Popconfirm
                        title="Delete"
                        description="Are you sure you want to delete this mantrap?"
                        onConfirm={() => clickDelete(id)}
                        icon={<ExclamationCircleFilled style={{color: "red"}}/>}
                    >
                        <Button disabled={!isAdmin} danger>Delete</Button>
                    </Popconfirm>
                </Space.Compact>
        }
    ]

    const loadMantraps = (values = {}) => {
        setLoading(true)
        getMantraps(values).then(response => {
            setLoading(false)
            if (response.status === 200)
                return setMantraps((response.body as GetMantraps).data)
            displayMessage.error(response.body.message).then()
        })
    }

    const clickAdd = () => {
        setOpen(true)
    }

    const clickEdit = (mantrapId: number) => {
        const user = mantraps.find(mantrap => mantrap.id === mantrapId)
        setSelected(user ?? null)
        setOpen(true)
    }

    const clickDelete = (mantrapId: number) => {
        deleteMantrap(mantrapId).then(response => {
            if (response.status === 200)
                return loadMantraps()
            displayMessage.error(response.body.message).then()
        })
    }

    const controlGate = (mantrapId: number, action: string) => {
        commandDoor(mantrapId, action).then(response => {
            if (response.status === 200) {
                displayMessage.success(response.body.message).then();
                loadMantraps(); // Refresh mantraps immediately after action
            } else {
                displayMessage.error(response.body.message).then();
            }
        });
    };

    return (
        <div className="p-8 flex flex-col gap-4 max-w-5xl m-auto">
            {contextHolder}
            <Typography>
                <Title level={2}>Manage Mantraps</Title>
            </Typography>
            <Form
                form={form}
                labelCol={{span: 8}}
                wrapperCol={{span: 16}}
                style={{maxWidth: 400}}
            >
                {/*<Form.Item*/}
                {/*  label="User ID"*/}
                {/*  name="id"*/}
                {/*>*/}
                {/*  <Input/>*/}
                {/*</Form.Item>*/}
                {/*<Form.Item*/}
                {/*  label="Username"*/}
                {/*  name="username"*/}
                {/*>*/}
                {/*  <Input/>*/}
                {/*</Form.Item>*/}
                {/*<Form.Item*/}
                {/*  label="Email"*/}
                {/*  name="email"*/}
                {/*>*/}
                {/*  <Input/>*/}
                {/*</Form.Item>*/}
                <Form.Item
                    label="Location"
                    name="location"
                >
                    <Input/>
                </Form.Item>
                <Form.Item
                    wrapperCol={{offset: 8, span: 16}}
                >
                    <Space>
                        <Button type="primary" onClick={clickSearch}>Search</Button>
                        <Button onClick={clickReset}>Reset</Button>
                    </Space>
                </Form.Item>
            </Form>
            <Button type="primary" className="self-end" onClick={clickAdd} disabled={!isAdmin}>Add Mantrap</Button>
            <Table
                dataSource={mantraps.map(mantrap => ({...mantrap, key: mantrap.id}))}
                columns={columns}
                pagination={false}
                loading={loading}
            />
            <MantrapModal
                open={open}
                setOpen={setOpen}
                selected={selected}
                setSelected={setSelected}
                reloadMantraps={loadMantraps}
                displayMessage={displayMessage}
            />
        </div>
    );
}