/* eslint-disable prettier/prettier */
import styled from '@emotion/styled'
import { useNavigate, useParams } from 'react-router-dom'
import ScreenContainerTop from '@ui/layouts/screen-container-top'
import ScreenHeading from '@ui/components/screen-heading'
import SimpleCardV1 from '@ui/components/cards/simple-card-v1'
import IconQrCode from '@icons/IconQrCode'
import IconKeyboardSettings from '@icons/IconKeyboardSettings'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  flex-direction: column;
  gap: 2rem;
  padding: 4rem 2rem 6rem 2rem;

  & .cards {
    padding: 2rem 0 0 0;
    display: flex;
    gap: 1.5rem;
  }
`

const SelectPickupMethodScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  const actions = {
    onBack: () => navigate(-1)
  }

  return (
    <ScreenContainerTop actions={actions}>
      <Container>
        <ScreenHeading
          top={() => <>{t('PICK UP PARCEL')}</>}
          middle={t('Pick up method')}
          bottom={() => (
            <>
              {t(
                'Do you want to enter code manually or scan the code from your mobile device screen?'
              )}
            </>
          )}
        />

        <div className="cards">
          <SimpleCardV1
            icon={(props) => <IconKeyboardSettings {...props} />}
            text={t('Enter code')}
            onClick={() => {
              navigate('/parcel-locker/pickup-enter-code')
            }}
          />
          <SimpleCardV1
            icon={(props) => <IconQrCode {...props} />}
            text={t('Scan code')}
            onClick={() => {
              navigate('/parcel-locker/pickup-scan-qr')
            }}
          />
        </div>
      </Container>
    </ScreenContainerTop>
  )
}

export default SelectPickupMethodScreen
