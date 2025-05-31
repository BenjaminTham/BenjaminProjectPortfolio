import {useForm} from "antd/lib/form/Form";
import {Form, Input, Modal} from "antd";
import {AddUser, GetMantraps} from "@/app/lib/types";
import {Dispatch, SetStateAction, useEffect} from "react";
import {addMantrap, updateMantrap} from "@/app/lib/api";
import {MessageInstance} from "antd/lib/message/interface";

export const MantrapModal = (
  {open, setOpen, selected, setSelected, reloadMantraps, displayMessage}: {open: boolean, setOpen: Dispatch<SetStateAction<boolean>>, selected: GetMantraps["data"][number] | null, setSelected: Dispatch<SetStateAction<GetMantraps["data"][number] | null>>, reloadMantraps: () => void, displayMessage: MessageInstance}
) => {

  const [form] = useForm()
  const addMode = selected === null

  const clickOk = () => {
    form.validateFields().then(values => {
      // Replace empty values with null
      values = Object.fromEntries(Object.entries(values).map(([key, value]) => [key, value !== "" ? value: null]));
      (addMode ? addMantrap(values): updateMantrap(selected.id, values)).then(response => {
        if (response.status === 200) {
          displayMessage.info(response.body.message).then()
          setOpen(false)
          setSelected(null)
          reloadMantraps()
          form.resetFields()
          return
        }
        // Show overall error message
        if (response.body.message)
          displayMessage.error(response.body.message).then()
        // Update errors for specific fields
        const errors = (response.body as AddUser).errors
        if (errors) {
          // eslint-disable-next-line @typescript-eslint/ban-ts-comment
          // @ts-expect-error
          form.setFields(Object.entries(errors).map(([key, value]) => ({name: key, errors: [value]})));
        }
      })
    })
  }

  useEffect(() => {
    if (!addMode) {
      form.setFields([
        {name: "location", value: selected.location}
      ])
    }
  }, [selected])



  return (
    <Modal
      title={addMode ? "Add Mantrap": "Edit Mantrap"}
      open={open}
      onOk={clickOk}
      onCancel={() => {
        setSelected(null)
        setOpen(false)
        form.resetFields()
      }}
      okText={addMode ? "Add": "Update"}
    >
      <Form
        form={form}
        labelCol={{span: 8}}
        wrapperCol={{span: 16}}
      >
        <Form.Item
          label="Location"
          name="location"
        >
          <Input/>
        </Form.Item>
      </Form>
    </Modal>
  )

}

export default MantrapModal