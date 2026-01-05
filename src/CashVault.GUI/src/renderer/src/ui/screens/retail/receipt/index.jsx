import styled from '@emotion/styled'
import Button from '@ui/components/button'
import IconCheckCircle from '@ui/components/icons/IconCheckCircle'
import IconCloseCircle from '@ui/components/icons/IconCloseCircle'
import IconReceipt from '@ui/components/icons/IconReceipt'
import RetailContainer from '@ui/components/retail-container'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  padding-top: 2rem !important;
  position: relative;
  display: flex;
  flex-direction: column;
  justify-content: space-between;

  width: 80%;
  margin: 0 auto;

  margin-bottom: 1.5rem;
  gap: 0 1.5rem;

  height: 70vh;

  & .heading {
    margin-top: 1.5rem;
    h1 {
      font-size: 4rem;
      line-height: 4.5rem;
      color: white;
      font-weight: 700;
      margin-bottom: 1.375rem;
    }
    p {
      font-size: 1.5rem;
      line-height: 2rem;
      font-weight: 400;
      color: var(--primary-light);
    }
  }

  & .repeat {
    width: 100%;

    & h2 {
      font-size: 2.5rem;
      line-height: 3rem;
      color: white;
      margin-bottom: 3rem;
    }

    & .buttons {
      display: flex;
      gap: 1.5rem;

      & .button {
        width: 100%;
      }
    }
  }
`

const ReceiptScreen = () => {
  const navigate = useNavigate()

  const handleHomeNavigate = () => {
    navigate('/retail/')
  }

  const handleIdleNavigate = () => {
    navigate('/retail/idle')
  }
  return (
    <RetailContainer
      color="secondary"
      hasBackButton={false}
      hasFaqButton={false}
      hasCartButton={false}
      hasHomeButton={false}
    >
      <Container>
        <div>
          <IconReceipt size="xl" color="var(--primary-light)" />
          <div className="heading">
            <h1>Please take your receipt</h1>

            <p>
              Your receipt is your proof of purchase. Take it to the cashier and pick up your order.
            </p>
          </div>
        </div>
        <div className="repeat">
          <h2>
            Would you like to continue <br /> shopping?
          </h2>

          <div className="buttons">
            <Button
              size="s"
              className="button"
              onClick={handleIdleNavigate}
              rounded="s"
              color="white"
              icon={(props) => <IconCloseCircle {...props} />}
            >
              No
            </Button>
            <Button
              size="s"
              className="button"
              onClick={handleHomeNavigate}
              rounded="s"
              color="secondary"
              icon={(props) => <IconCheckCircle {...props} />}
            >
              Yes
            </Button>
          </div>
        </div>
      </Container>
    </RetailContainer>
  )
}

export default ReceiptScreen
