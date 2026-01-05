/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import DropdownInput from '@ui/components/inputs/dropdown-input'
import CheckboxInput from '@ui/components/inputs/checkbox-input'
import { useTranslation } from '@domain/administration/stores'

const Container = styled.div`
  display: flex;
  align-items: center;
  gap: 1.5rem;

  & .item {
    border-left: 2px solid var(--primary-medium);
    display: flex;
    align-items: center;
    justify-content: center;
    height: 3rem;
    padding-left: 1rem;
    margin-top: 1.5rem;
  }

  border-bottom: 1px solid var(--primary-medium);
  padding: 1rem 0;
`

const CassetteItem = ({
  cassette,
  disabled,
  onDenomMagnetChange = () => {},
  onDenomChange = () => {},
  supportedDenominations
}) => {
  const { t } = useTranslation()
  return (
    <Container>
      <DropdownInput
        label={`${t('Cassette')} #${cassette.cassetteNumber} - ${t('Value')}`}
        disabled={disabled}
        options={supportedDenominations?.map((x) => ({
          value: x.value,
          name: x.value.toString()
        }))}
        value={cassette.billDenomination.value}
        onChange={onDenomChange}
      />
      <div className="item">
        <CheckboxInput
          label="A"
          value={cassette?.denominationMagnetStatus?.magnetA || false}
          disabled={disabled}
          onChange={(e) =>
            onDenomMagnetChange(cassette.cassetteNumber, 'magnetA', e.target.checked)
          }
        />
      </div>
      <div className="item">
        <CheckboxInput
          label="B"
          value={cassette?.denominationMagnetStatus?.magnetB || false}
          disabled={disabled}
          onChange={(e) =>
            onDenomMagnetChange(cassette.cassetteNumber, 'magnetB', e.target.checked)
          }
        />
      </div>
      <div className="item">
        <CheckboxInput
          label="C"
          value={cassette?.denominationMagnetStatus?.magnetC || false}
          disabled={disabled}
          onChange={(e) =>
            onDenomMagnetChange(cassette.cassetteNumber, 'magnetC', e.target.checked)
          }
        />
      </div>
      <div className="item">
        <CheckboxInput
          label="D"
          value={cassette?.denominationMagnetStatus?.magnetD || false}
          disabled={disabled}
          onChange={(e) =>
            onDenomMagnetChange(cassette.cassetteNumber, 'magnetD', e.target.checked)
          }
        />
      </div>
    </Container>
  )
}

export default CassetteItem
