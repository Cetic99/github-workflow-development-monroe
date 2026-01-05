/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useRef, useState } from 'react'
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import FullPageLoader from '@ui/components/full-page-loader'
import Alert from '@ui/components/alert'
import Modal from '@ui/components/modal'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  overflow: hidden;
  height: 100%;

  .exec-title {
    display: flex;
    gap: 2rem;
    align-items: center;

    & .spinner {
      margin-left: auto;
    }
  }

  & .content {
    overflow-y: auto;
    border: 1px solid gray;
    border-radius: 0.25rem;
    padding: 0.5rem;
    max-height: 500px;
    background: black;
    font-size: 0.875rem;
  }

  & .output {
    display: flex;
    flex-direction: column;
    gap: 0.35rem;
  }
`
const ModalContent = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
`

const ScriptFile = ({ scriptName, onSelect }) => {
  const [executing, setExecuting] = useState(false)
  const [result, setResult] = useState(false)
  const { t } = useTranslation()
  const modalRef = useRef(null)

  const onExecute = async () => {
    modalRef.current?.showModal()

    setExecuting(true)

    setTimeout(async () => {
      const output = await onSelect()
      setResult(output)
      setExecuting(false)
    }, 2000)
  }

  const onClose = () => {
    setExecuting(false)
    setResult('')
    modalRef.current?.close()
  }

  return (
    <>
      <Alert severity="info" onClick={onExecute} buttonText={t('Run')} />
      <Modal ref={modalRef} onClose={onClose}>
        <ModalContent>
          <h1>{scriptName}</h1>

          <Container>
            <FullPageLoader loading={executing} message={t('Running script...')} />

            {result && (
              <div className="output">
                <div>{t('Output:')}</div>
                <div className="content">{result}</div>
              </div>
            )}
          </Container>
        </ModalContent>
      </Modal>
    </>
  )
}

export default ScriptFile
