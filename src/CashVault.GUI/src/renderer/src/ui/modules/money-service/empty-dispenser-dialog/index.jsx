/* eslint-disable react/display-name */
/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import { useTranslation } from '@domain/administration/stores'
import Modal from '@ui/components/modal'
import { forwardRef, useEffect, useState } from 'react'
import styled from '@emotion/styled'
import DecimalInput from '@ui/components/inputs/decimal-input'
import Button from '@ui/components/button'
import CheckboxInput from '@ui/components/inputs/checkbox-input'

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

  display: flex;
  gap: 2rem;

  & .cassette-item {
    flex-grow: 100;
  }
`

const EmptyDispenserDialog = forwardRef((props, ref) => {
  const {
    cassettes = [],
    onSave = () => {},
    onClose = () => {},
    dispenserRejectedBillsCount = 0
  } = props

  const { t } = useTranslation()
  const [cassettesConfig, setCassettesConfig] = useState([])
  const [emptyRejectBin, setEmptyRejectBin] = useState(false)

  const handleOnSave = () => {
    const cassettesNumbers = cassettesConfig
      ?.filter((x) => x?.checked === true)
      .map((x) => x.cassetteNumber)

    onSave({ cassettesNumbers: cassettesNumbers, emptyRejectBin })
    ref?.current?.close()
  }

  useEffect(() => {
    if (cassettes?.length > 0) {
      var sortedCassettes = cassettes?.sort((a, b) => a?.cassetteNumber - b?.cassetteNumber)
      setCassettesConfig(sortedCassettes)
    }
  }, [cassettes])

  //=======================================================
  const handleCassetteChange = (name, value) => {
    setCassettesConfig((prev) =>
      prev.map((cassette) => {
        if (cassette?.name === name) {
          return { ...cassette, checked: value }
        }
        return cassette
      })
    )
  }

  return (
    <Modal ref={ref} onClose={onClose}>
      <Container>
        <div className="header">{t('Empty dispenser')}</div>

        <div className="form">
          {cassettesConfig?.map((x, i) => (
            <Casssette key={`${x?.name}__${i}`} hasBorder={i > 0}>
              <CheckboxInput
                value={x?.checked || false}
                onChange={(e) => handleCassetteChange(x.name, e?.target?.checked)}
              />
              <div className="cassette-item">
                <p className="cassette-title">{x.name}</p>
                <DecimalInput
                  size="m"
                  value={x.billCount}
                  hasClear={false}
                  disabled={true}
                  hasClearButton={false}
                  valuePosition="left"
                />
              </div>
            </Casssette>
          ))}
          <Casssette hasBorder={true}>
            <CheckboxInput
              value={emptyRejectBin || false}
              onChange={(e) => setEmptyRejectBin(e?.target?.checked)}
            />
            <div className="cassette-item">
              <DecimalInput
                disabled={true}
                size="m"
                label={t('Reject bin')}
                value={dispenserRejectedBillsCount}
                hasClear={false}
                hasClearButton={false}
                valuePosition="left"
              />
            </div>
          </Casssette>
        </div>

        <div className="actions">
          <Button color="light" onClick={onClose}>
            Cancel
          </Button>

          <Button
            confirmAction={true}
            onClick={handleOnSave}
            disabled={cassettesConfig?.filter((x) => x?.checked === true)?.length === 0 && !emptyRejectBin}
          >
            Save
          </Button>
        </div>
      </Container>
    </Modal>
  )
})

export default EmptyDispenserDialog
