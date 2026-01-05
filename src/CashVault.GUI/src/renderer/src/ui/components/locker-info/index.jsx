/* eslint-disable react/prop-types */
/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import IconLocation from '@icons/IconLocation'
import { useTranslation } from '@src/app/domain/administration/stores'
import { useLocationAddress, useMachineName } from '@src/app/domain/administration/stores/regional'

const Container = styled.div`
  padding: 1rem 1.5rem 1rem 1rem;
  background-color: transparent;
  width: fit-content;
  display: flex;
  gap: 0.75rem;
  align-items: center;

  & .icon-container {
    border: 1px solid var(--secondary-dark);
    padding: 1rem;
    border-radius: 100%;
    display: flex;
    align-items: center;
    justify-content: center;
    width: 4rem;
    height: 4rem;
  }

  & .content {
    display: flex;
    flex-direction: column;

    & .main {
      font-weight: 700;
      font-style: Bold;
      font-size: 1.5rem;
      line-height: 2rem;
      color: var(--primary-light);
    }

    & .secondary {
      font-weight: 600;
      font-style: SemiBold;
      font-size: 0.75rem;
      line-height: 1.125rem;
      text-transform: uppercase;
      color: white;
    }
  }
`

const LockerInfo = (props) => {
  const locationAddress = useLocationAddress()
  const machineName = useMachineName()

  const { t } = useTranslation()

  return (
    <Container>
      <div className="icon-container">
        <IconLocation size="m" color="white" />
      </div>

      <div className="content">
        <span className="main">{locationAddress}</span>
        <span className="secondary">{`${t('LOCKER ID')}: ${machineName}`}</span>
      </div>
    </Container>
  )
}

export default LockerInfo
