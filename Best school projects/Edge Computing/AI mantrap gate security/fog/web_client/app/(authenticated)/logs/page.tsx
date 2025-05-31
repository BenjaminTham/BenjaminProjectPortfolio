"use client"

import {Button, Form, Input, message, Space, Table, Typography, Image} from "antd";
import {useForm} from "antd/lib/form/Form";
import {getLogs} from "@/app/lib/api";
import {useEffect, useState} from "react";
import {GetLogs} from "@/app/lib/types";
const { Title } = Typography;


export default function Users() {

  const [form] = useForm()
  const [logs, setLogs] = useState<GetLogs["data"]>([])
  const [displayMessage, contextHolder] = message.useMessage()
  const [loading, setLoading] = useState(false)
  const [image, setImage] = useState("")
  const [imageVisible, setImageVisible] = useState(false)

  useEffect(() => {
    loadLogs()
  }, [])

  const clickSearch = () => {
    form.validateFields().then(values => {
      if (values.user_id)
        values["user_id"] = parseInt(values.user_id)
      loadLogs(values)
    })
  }

  const clickReset = () => {
    form.resetFields()
    loadLogs()
  }

  const columns = [
    {title: "ID", dataIndex: "id"},
    {title: "Category", dataIndex: "category"},
    {title: "User ID", dataIndex: "user_id"},
    // {title: "Mantrap ID", dataIndex: "mantrap_id"},
    {title: "Message", dataIndex: "message"},
    {title: "Timestamp", dataIndex: "timestamp"},
    {title: "File", dataIndex: "file", render: (filename: string) => filename ? <Button onClick={() => clickViewImage(filename)}>View Image</Button>: ""}
  ]

  const loadLogs = (values = {}) => {
    setLoading(true)
    getLogs(values).then(response => {
      setLoading(false)
      if (response.status === 200)
        return setLogs((response.body as GetLogs).data)
      displayMessage.error(response.body.message).then()
    })
  }

  const clickViewImage = (filename: string) => {
    setImage(process.env.NEXT_PUBLIC_API_URL + "/log/file/" + filename)
    setImageVisible(true)
  }

  return (
    <div className="p-8 flex flex-col gap-4 max-w-5xl m-auto">
      {contextHolder}
      <Typography>
        <Title level={2}>System Logs</Title>
      </Typography>
      <Form
        form={form}
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
        style={{maxWidth: 400}}
      >
        <Form.Item
          label="Category"
          name="category"
        >
          <Input/>
        </Form.Item>
        <Form.Item
          label="User ID"
          name="user_id"
        >
          <Input/>
        </Form.Item>
        {/*<Form.Item*/}
        {/*  label="Mantrap ID"*/}
        {/*  name="mantrap_id"*/}
        {/*>*/}
        {/*  <Input/>*/}
        {/*</Form.Item>*/}
        <Form.Item
          wrapperCol={{offset: 8, span: 16}}
        >
          <Space>
            <Button type="primary" onClick={clickSearch}>Search</Button>
            <Button onClick={clickReset}>Reset</Button>
          </Space>
        </Form.Item>
      </Form>
      <Table
        dataSource={logs.map(log => ({...log, key: log.id}))}
        columns={columns}
        pagination={false}
        loading={loading}
      />
      {image ?
        <Image
          src={image}
          style={{display: "none"}}
          placeholder={true}
          preview={{
            visible: imageVisible,
            onVisibleChange: (visible: boolean) => setImageVisible(visible),
            destroyOnClose: true
          }}
        />: ""
      }
      {/*<UserModal*/}
      {/*  open={open}*/}
      {/*  setOpen={setOpen}*/}
      {/*  selected={selected}*/}
      {/*  setSelected={setSelected}*/}
      {/*  reloadUsers={loadUsers}*/}
      {/*  displayMessage={displayMessage}*/}
      {/*/>*/}
    </div>
  );
}