/* eslint-disable prettier/prettier */
/* eslint-disable react/prop-types */
import styled from '@emotion/styled'
import { useTranslation } from '@domain/administration/stores'
import Alert from '@ui/components/alert'

const Container = styled.div`
  padding: 1rem 0;
  animation: fadeIn 0.5s ease-in-out;
  gap: 0.5rem;
  display: flex;
  flex-direction: column;
  z-index: 99;
  margin-top: 7.5rem;

  & .divider {
    height: 2px;
    background: var(--bg-divider);
    border-radius: 2rem;
    margin-top: 0.25rem;
    margin: 0.25rem;
    margin-left: 0.5rem;
    margin-right: 0.5rem;
  }

  @keyframes fadeIn {
    from {
      opacity: 0;
    }
    to {
      opacity: 1;
    }
  }
`

const DeviceWarnings = ({
  showPrinterWarning = false,
  showBillDispenserWarning = false,
  showBillAcceptorWarning = false
}) => {
  const { t } = useTranslation()

  return (
    <>
      {(showBillAcceptorWarning === true ||
        showBillDispenserWarning === true ||
        showPrinterWarning === true) && (
        <Container>
          {showBillDispenserWarning === true && (
            <Alert severity="warning" text={t('Bill dispenser does not work properly.')} showModal={false} />
          )}
          {showBillAcceptorWarning === true && (
            <Alert severity="warning" text={t('Bill acceptor does not work properly.')} showModal={false} />
          )}
          {showPrinterWarning === true && (
            <Alert severity="warning" text={t('Ticket printer does not work properly.')} showModal={false} />
          )}
          <div className="divider"></div>
          <Alert variant="outlined" severity="error" text={t('Please contact support.')} showModal={false} />
        </Container>
      )}
    </>
  )
}

export default DeviceWarnings
