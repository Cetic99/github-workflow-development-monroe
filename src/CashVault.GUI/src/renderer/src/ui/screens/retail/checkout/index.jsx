import styled from '@emotion/styled'
import Button from '@ui/components/button'
import CardButton from '@ui/components/card-button'
import IconBankCard from '@ui/components/icons/IconBankCard'
import IconBanknote01 from '@ui/components/icons/IconBanknote01'
import IconCloseCircle from '@ui/components/icons/IconCloseCircle'
import IconCoupon from '@ui/components/icons/IconCoupon'
import RetailContainer from '@ui/components/retail-container'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;
  margin-bottom: 1.5rem;
  gap: 0 1.5rem;

  height: 100%;

  .header {
    display: flex;
    flex-direction: column;
    align-items: center;
    width: 100%;
    h1 {
      font-size: 4rem;
      line-height: 4.5rem;
      font-weight: 700;
      margin-bottom: 1.5rem;
    }
    p {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;

      & span {
        font-weight: 700;
        color: var(--secondary-dark);
      }
    }
    margin-bottom: 10.25rem;
  }

  & .methods {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 1.5rem;
  }
`

const Footer = styled.div`
  position: absolute;
  bottom: 0;
  left: 0;
  width: 100%;
  height: 9rem;
  background-color: white;

  & .wrapper {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }

  gap: 0.5rem;
  & .wave-svg {
    position: absolute;
    top: -3rem;
    left: 0;
  }

  & .amount {
    padding-bottom: 1rem;

    h2 {
      font-size: 2rem;
      line-height: 2.5rem;
      margin-bottom: 0.75rem;
      font-weight: 600;
    }

    h1 {
      font-size: 4rem;
      line-height: 4.5rem;
      font-weight: 700;
    }
  }

  & .buttons {
    padding-bottom: 1rem;

    display: flex;
    height: fit-content;
    gap: 1.5rem;
  }
`

const Wave = () => {
  return (
    <svg
      xmlns="http://www.w3.org/2000/svg"
      fill="none"
      className="wave-svg"
      viewBox="0 0 327.68 54.26190476190476"
      width="327.68"
      height="54.26190476190476"
    >
      <path
        fill="white"
        d="M185.852 0C204.759 2.20537e-05 222.734 8.19802 235.215 22.5059L240.522 28.5986C254.411 44.5152 274.065 55.0234 295.092 55.0234H303V55H1036C1044.84 55 1052 62.1634 1052 71V484C1052 492.837 1044.84 500 1036 500H5C-3.83656 500 -11 492.837 -11 484V16C-11 7.16345 -3.83655 2.09384e-07 5 0H185.852Z"
      />
    </svg>
  )
}

const CheckoutScreen = () => {
  const navigate = useNavigate()

  const handleMethodClick = (value) => {
    navigate(`/retail/${value}-payment`)
  }
  return (
    <RetailContainer>
      <Container>
        <div className="header">
          <h1>Checkout</h1>
          <p>Please select the payment method for this purchase.</p>
          <p>
            Payment amount: <span>€94.98</span>
          </p>
        </div>

        <div className="methods">
          <CardButton
            text={'Cash'}
            icon={<IconBanknote01 size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleMethodClick('cash')}
          />
          <CardButton
            text={'Coupon'}
            icon={<IconCoupon size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleMethodClick('coupon')}
          />
          <CardButton
            text={'Card'}
            icon={<IconBankCard size="l" />}
            color="white"
            activeColor="#f0f0f0"
            textColor="black"
            onClick={() => handleMethodClick('card')}
          />
        </div>
      </Container>
      <Footer>
        <Wave color="#FFFFFF" />
        <h2>Subtotal:</h2>
        <div className="wrapper">
          <div className="amount">
            <h1>€94.98</h1>
          </div>
          <div className="buttons">
            <Button
              size="s"
              rounded="s"
              color="white"
              icon={(props) => <IconCloseCircle {...props} />}
            >
              Cancel order
            </Button>
          </div>
        </div>
      </Footer>
    </RetailContainer>
  )
}

export default CheckoutScreen
