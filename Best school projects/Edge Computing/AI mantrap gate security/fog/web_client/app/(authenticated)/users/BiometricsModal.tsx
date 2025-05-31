import {Button, Divider, Modal, Space, Typography} from "antd";
import {Dispatch, SetStateAction, useEffect, useRef, useState} from "react";
import {GetUsers} from "@/app/lib/types";
import {MessageInstance} from "antd/lib/message/interface";
import Title from "antd/lib/typography/Title";
import {CameraOutlined, DeleteOutlined} from "@ant-design/icons";
import {convertToWav} from "@/app/lib/wav_generator";
import {enrollBiometrics} from "@/app/lib/api";

const {Text} = Typography;

export const BiometricsModal = (
    {open, setOpen, selected, setSelected, reloadUsers, displayMessage}: {
        open: boolean,
        setOpen: Dispatch<SetStateAction<boolean>>,
        selected: GetUsers["data"][number] | null,
        setSelected: Dispatch<SetStateAction<GetUsers["data"][number] | null>>,
        reloadUsers: () => void,
        displayMessage: MessageInstance
    }
) => {

    const videoRef = useRef<HTMLVideoElement | null>(null)
    const canvasRef = useRef<HTMLCanvasElement | null>(null)
    const mediaRecorderRef = useRef<MediaRecorder | null>(null)
    const audioChunksRef = useRef<Blob[]>([])
    const timerRef = useRef<NodeJS.Timeout>(null)
    const [seconds, setSeconds] = useState(0)
    const [images, setImages] = useState<string[]>([])
    const [audio, setAudio] = useState<string | undefined>(undefined);
    const [isRecording, setIsRecording] = useState(false)
    const [loading, setLoading] = useState(false)

    const clickOk = async () => {

        setLoading(true)

        if (images.length != 5 || !audio || selected === null) {
            displayMessage.error("5 images and a secure voice password are required for enrollment.").then()
            return
        }

        // Get back all the blobs
        const imageBlobs = await Promise.all(images.map(async imageUrl => (await fetch(imageUrl)).blob()))
        const audioBlob = await (await fetch(audio)).blob()

        const formData = new FormData()
        imageBlobs.forEach((blob, _) => formData.append(`face_${_}`, blob, `image_${_}.png`))
        formData.append("voice", audioBlob, "audio.wav")

        enrollBiometrics(selected.id, formData).then(response => {
            setLoading(false)

            if (response.status === 200) {
                displayMessage.success(response.body.message).then()
                setOpen(false)
                setSelected(null)
                reloadUsers()
                setImages([])
                setAudio(undefined)
                setSeconds(0)
                return
            }

            if (response.body.message)
                displayMessage.error(response.body.message).then()
        })


    }

    useEffect(() => {
        if (!open) return

        // Add camera stream to video object
        navigator.mediaDevices.getUserMedia({video: true}).then(stream => {
            if (videoRef.current)
                videoRef.current.srcObject = stream
        })

        // Cleanup on close
        return () => {
            if (videoRef.current?.srcObject)
                (videoRef.current.srcObject as MediaStream).getTracks().forEach(track => track.stop())
        }
    }, [open])

    const captureImage = () => {
        const canvas = canvasRef.current
        const video = videoRef.current
        const context = canvas?.getContext("2d")

        if (!(canvas && video && context))
            return

        canvas.width = video.videoWidth
        canvas.height = video.videoHeight
        context.drawImage(video, 0, 0, video.videoWidth, video.videoHeight)
        canvas.toBlob(blob => {
            if (blob)
                setImages([...images, URL.createObjectURL(blob)])
        }, "image/png")
    }

    const deleteImage = (index: number) => {
        setImages(images.filter((_image, _) => _ !== index))
    }

    const startRecording = () => {
        setIsRecording(true)

        navigator.mediaDevices.getUserMedia({audio: true}).then(stream => {
            const mediaRecorder = new MediaRecorder(stream)
            mediaRecorderRef.current = mediaRecorder

            // Save audio chunks
            mediaRecorder.ondataavailable = event => {
                if (event.data.size > 0)
                    audioChunksRef.current.push(event.data)
            }

            // Set stop callback
            mediaRecorder.onstop = async () => {
                const audioBlob = new Blob(audioChunksRef.current, {type: "audio/pcm"})
                const wavBlob = await convertToWav(audioBlob)
                const audioUrl = URL.createObjectURL(wavBlob)
                setAudio(audioUrl)
                audioChunksRef.current = []
            }

            mediaRecorder.start()

            // Update timer and stop at 3 seconds
            timerRef.current = setInterval(() => {
                setSeconds(prev => {
                    if (prev >= 3) {
                        stopRecording();
                        return prev; // Prevents unnecessary re-render
                    }
                    return prev + 1;
                });
            }, 1000);
        })
    }

    const stopRecording = () => {
        setIsRecording(false)
        mediaRecorderRef.current?.stop()

        // Clear timer
        if (timerRef.current)
            clearInterval(timerRef.current)
    }

    const deleteRecording = () => {
        setAudio(undefined)
        setSeconds(0)
    }


    return (
        <Modal
            title="Enroll Biometrics"
            open={open}
            onOk={clickOk}
            okButtonProps={{loading: loading, disabled: loading}}
            onCancel={() => {
                setSelected(null)
                setOpen(false)
                setImages([])
                setAudio(undefined)
                setSeconds(0)
            }}
            width={{
                xs: '80%',
                sm: '70%',
                md: '60%',
                lg: '50%',
                xl: '40%',
                xxl: '30%',
            }}
            okText="Enroll"
        >
            <div className="flex flex-col py-4 gap-4">
                <Space direction="vertical">
                    <Text>Enrolling biometrics for <Text strong>{selected?.username}</Text></Text>
                </Space>
                <div>
                    <Title level={5}>Face Enrollment</Title>
                    <Text>Capture 5 images of your face at different angles.</Text>
                    <Divider style={{marginTop: 8, marginBottom: 0}}/>
                </div>
                <div className="flex flex-col gap-2">
                    <div className="relative">
                        <video ref={videoRef} autoPlay style={{transform: "scaleX(-1)", width: "100%"}}/>
                        <div
                            className={`absolute z-200 bottom-4 left-1/2 -translate-x-1/2 ${images.length >= 5 ? "hidden" : ""}`}>
                            <Button type="primary" shape="circle" icon={<CameraOutlined/>} onClick={captureImage}
                                    size="large"/>
                        </div>
                    </div>
                    <canvas ref={canvasRef} style={{display: "none"}}/>
                    <div className="flex items-center">
                        {Array.from({length: 5}).map((_, i) => (
                            <div key={i} className="relative flex-1">
                                <img
                                    src={images.length > i ? images[i] : "data:image/png;base64,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"}
                                    alt="photo"
                                    style={{objectFit: "cover", width: "100%", aspectRatio: 1}}
                                />
                                {images.length > i ?
                                    <div className="absolute top-2 right-2">
                                        <Button type="primary" icon={<DeleteOutlined/>} size="small"
                                                onClick={() => deleteImage(i)} danger/>
                                    </div> : <></>
                                }
                            </div>
                        ))}
                    </div>
                </div>
                <div>
                    <Title level={5}>Voice Enrollment</Title>
                    <Text>Record a secure 3 second voice password.</Text>
                    <Divider style={{marginTop: 8, marginBottom: 0}}/>
                </div>
                <div className="flex gap-8 items-center">
                    {audio === undefined ?
                        (!isRecording ?
                                <Button type="primary" onClick={startRecording}>Start Recording</Button> :
                                <Button type="primary" onClick={stopRecording}>Stop Recording</Button>
                        ) :
                        <Button type="primary" onClick={deleteRecording} danger>Delete Recording</Button>
                    }

                    <Text>00:{String(seconds).padStart(2, "0")}/00:03</Text>
                </div>
                <audio controls src={audio} key={audio} controlsList="noplaybackrate nodownload" preload="auto"/>
            </div>
        </Modal>
    )

}

