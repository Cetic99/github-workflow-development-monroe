/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import CircleButton from '@ui/components/circle-button'
import IconClose from '@ui/components/icons/IconClose'
import IconPlus from '@ui/components/icons/IconPlus'
import IconMinus from '@ui/components/icons/IconMinus'
import DecimalInput from '@ui/components/inputs/decimal-input'
import BillDenominationSing from '@ui/components/bill-denomination-sign'

const Container = styled.div`
  display: flex;
  align-items: center;
  gap: 1rem;

  & .bill-coin-amount {
    max-width: 10rem;
  }

  & .c-value {
    display: flex;
    margin-left: auto;
    align-items: center;
    gap: 0.5rem;

    font-weight: 700;
    font-size: 2.125rem;
    line-height: 3.75rem;
    letter-spacing: -4%;
    text-align: right;

    & span {
      font-weight: 500;
      font-size: 2.125rem;
      line-height: 2.375rem;
      letter-spacing: -3%;
      text-align: right;
    }
  }
`

const DenominationConfig = ({
  denom,
  denomConfig,
  count = 0,
  currency = 'BAM',
  increaseDisabled = false,
  decreaseDisabled = false,
  onClear = () => {},
  onIncrease = () => {},
  onDecrease = () => {},
  onChange = () => {}
}) => {
  return (
    <Container>
      <div className="denom">
        <BillDenominationSing value={denom} />
      </div>

      <IconClose size="l" color="var(--primary-dark)" />

      <div className="bill-coin-amount">
        <DecimalInput
          size="m"
          value={count}
          onClear={() => onClear(denomConfig)}
          onValueChange={(value) => onChange(denomConfig, parseInt(value))}
        />
      </div>

      <div className="separator" />

      <CircleButton
        size="s"
        icon={(props) => <IconMinus {...props} />}
        disabled={decreaseDisabled}
        onClick={() => onDecrease(denomConfig)}
      />
      <CircleButton
        size="s"
        icon={(props) => <IconPlus {...props} />}
        disabled={increaseDisabled}
        onClick={() => onIncrease(denomConfig)}
      />

      <div className="separator" />

      <div className="c-value">
        {(count * denom).toFixed(2)}
        <span>{currency}</span>
      </div>
    </Container>
  )
}

export default DenominationConfig
