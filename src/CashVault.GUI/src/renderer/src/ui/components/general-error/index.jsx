/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { restartApplication } from '@src/app/infrastructure/process'
import Button from '@ui/components/button'
import IconRefresh from '../icons/IconRefresh'

const Container = styled.div`
  position: relative;
  width: 100dvw;
  height: 100dvh;
  display: flex;
  justify-content: center;
  align-items: center;

  & .content {
    text-align: center;
    display: flex;
    flex-direction: column;
    gap: 0.25rem;
    justify-content: center;
    align-items: center;
  }

  & .action {
    padding-top: 1rem;
  }
`

const GeneralError = () => {
  return (
    <Container>
      <div className="content">
        <div>Something went wrong, please contact support</div>
        <div>or try restarting the application</div>

        <div className="action">
          <Button
            variant="contained"
            size="large"
            startIcon={<IconRefresh />}
            onClick={() => {
              restartApplication()
            }}
          >
            Restart
          </Button>
        </div>
      </div>
    </Container>
  )
}

export default GeneralError
