import ScreenHeading from '@ui/components/screen-heading'
import styled from '@emotion/styled'
import IconLockPin from '@ui/components/icons/IconLockPin'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import PinInput from '@ui/components/inputs/pin-input'
import { useNavigate } from 'react-router-dom'

const Container = styled.div`
  padding: 4rem 2rem 6rem 2rem;
`

const PinInputScreen = () => {
  const navigate = useNavigate()

  const actions = {
    onBack: () => {},
    onProceed: () => {
      navigate('/atm/')
    }
  }

  const handleComplete = () => {
    // TODO: Logic for completion of pin input
  }

  return (
    <ScreenContainerTop actions={actions} supportVisible={false} infoVisible={false}>
      <Container>
        <ScreenHeading
          top={() => <IconLockPin size="xl" color="#020605" />}
          middle="Please enter your PIN"
          bottom={() =>
            'After PIN entry tap the ‘Proceed’ button. In case of three invalid entries, your card will be blocked.'
          }
          bottomColor="var(--primary-dark)"
        />
        <PinInput length={4} onComplete={handleComplete} />
      </Container>
    </ScreenContainerTop>
  )
}
export default PinInputScreen
