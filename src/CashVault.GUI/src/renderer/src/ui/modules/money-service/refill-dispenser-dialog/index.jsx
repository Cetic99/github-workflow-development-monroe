/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import Modal from '@ui/components/modal'
import { forwardRef, useEffect, useImperativeHandle, useRef, useState } from 'react'
import styled from '@emotion/styled'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Button from '@ui/components/button'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  height: 100%;

  & .header {
    font-weight: 600;
    font-size: 1.75rem;
    line-height: 1.875rem;
    margin-bottom: 1rem;
  }

  & .form {
    overflow-y: auto;
    display: flex;
    flex-direction: column;
  }

  & .actions {
    display: flex;
    gap: 1rem;
  }

  & .actions button:nth-of-type(2) {
    margin-left: auto;
  }

  & .cassette-title {
    font-size: 1rem;
    line-height: 1.75rem;
    text-transform: uppercase;
    font-weight: 600;
    color: var(--primary-medium);
  }
`

const Casssette = styled.div`
  padding-bottom: 0.75rem;
  padding-top: 0.75rem;
  border-top: ${(p) => (p.hasBorder ? '1px' : '0')} solid var(--primary-medium);
`

const RefillDispenserDialog = forwardRef(
  ({ cassettes = [], onClose = () => {}, onSave = () => {} }, ref) => {
    const firstInputRef = useRef()
    const modalRef = useRef()
    const { t } = useTranslation()
    const [cassettesConfig, setCassettesConfig] = useState([])

    const handleOnSave = () => {
      const cassettesList = cassettesConfig.map((element) => ({
        cassetteNumber: element.cassetteNumber,
        billCount: Number(element?.billCount || 0)
      }))

      onSave({ cassettes: cassettesList })
      modalRef.current?.close()
    }

    useEffect(() => {
      if (cassettes?.length > 0) {
        const temp = (cassettes ?? []).map((x) => ({ ...x, billCount: 0 }))
        var sortedCassettes = temp?.sort((a, b) => a?.cassetteNumber - b?.cassetteNumber)
        setCassettesConfig(sortedCassettes)
      }
    }, [cassettes])

    //=======================================================
    const handleCassetteChange = (name, value) => {
      setCassettesConfig((prev) =>
        prev.map((cassette) => {
          if (cassette?.name === name) {
            return { ...cassette, billCount: value }
          }
          return cassette
        })
      )
    }

    useImperativeHandle(ref, () => ({
      showModal: () => {
        modalRef.current?.showModal()

        setTimeout(() => {
          firstInputRef.current?.focus()
        }, 150)
      },
      close: () => {
        modalRef.current?.close()
      }
    }))

    return (
      <Modal ref={modalRef} onClose={onClose}>
        <Container>
          <div className="header">{t('Refill dispenser')}</div>
          <div className="form">
            {cassettesConfig?.map((x, i) => (
              <Casssette key={`${x?.name}__${i}`} hasBorder={i > 0}>
                <div>
                  <p className="cassette-title">{x.name}</p>
                  <DecimalInput
                    ref={i === 0 ? firstInputRef : null}
                    size="m"
                    value={x.billCount}
                    hasClear={false}
                    allowNegativeValue={false}
                    allowDecimals={false}
                    onValueChange={(value) => handleCassetteChange(x.name, value)}
                    onClear={() => handleCassetteChange(x.name, 0)}
                    hasClearButton={false}
                    valuePosition="left"
                  />
                </div>
              </Casssette>
            ))}
          </div>
          <div className="actions">
            <Button color="light" onClick={onClose}>
              Cancel
            </Button>

            <Button confirmAction={true} onClick={handleOnSave}>
              Save
            </Button>
          </div>
        </Container>
      </Modal>
    )
  }
)

export default RefillDispenserDialog
