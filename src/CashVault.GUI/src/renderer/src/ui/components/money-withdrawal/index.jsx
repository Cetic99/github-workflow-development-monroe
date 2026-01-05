import styled from '@emotion/styled'
import CardButton from '@ui/components/card-button'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import { useNavigate } from 'react-router-dom'
const Container = styled.div`
  h2 {
    font-weight: 600;
    font-size: 1.75rem;
    margin-bottom: 2rem;
  }
`

const Options = styled.div`
  display: grid;
  gap: 1.25rem;
  height: 20.625rem;

  grid-template-columns: repeat(4, 1fr);
  grid-template-rows: repeat(2, 1fr);

  .last-withdrawal {
    grid-row: span 2;
  }
`

const MoneyWithdrawal = ({ moneyOptions = [], lastWithdrawalAmount = 0 }) => {
  const navigate = useNavigate()
  const handleAmountClicked = () => {
    //TODO: Implement on click amount
    navigate('/atm/take-card/money')
  }

  const handleChooseAmountClicked = () => {
    navigate('/atm/choose-amount')
  }

  return (
    <Container>
      <h2>Money withdrawal</h2>

      <Options>
        <CardButton
          className="last-withdrawal"
          text={`${lastWithdrawalAmount} €`}
          subtitle="Your last withdrawal"
          icon={<IconBanknote01 size="l" />}
          color="white"
          activeColor="#f0f0f0"
          onClick={handleAmountClicked}
          textColor="black"
        />
        {moneyOptions.map((option, index) => (
          <CardButton
            text={`${option} €`}
            key={index}
            icon={<IconBanknote01 size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={handleAmountClicked}
          />
        ))}
        <CardButton
          text="Choose amount"
          overline="OR"
          hasGap={false}
          onClick={handleChooseAmountClicked}
        />
      </Options>
    </Container>
  )
}

export default MoneyWithdrawal
