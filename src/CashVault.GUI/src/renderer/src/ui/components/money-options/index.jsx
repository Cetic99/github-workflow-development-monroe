import styled from '@emotion/styled'
import CardButton from '@ui/components/card-button'
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

const MoneyOptions = ({ options = [] }) => {
  const navigate = useNavigate()

  const handleMoneyOptionClick = (option) => {
    navigate(option.route)
  }

  return (
    <Container>
      <h2>Monroe & More</h2>

      <Options>
        {options.map((option, index) => (
          <CardButton
            text={`${option.title}`}
            key={index}
            icon={option.icon}
            onClick={() => handleMoneyOptionClick(option)}
          />
        ))}
      </Options>
    </Container>
  )
}

export default MoneyOptions
