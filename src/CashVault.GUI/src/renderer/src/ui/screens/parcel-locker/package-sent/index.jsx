import styled from '@emotion/styled'
import ScreenContainerBottom from '@ui/layouts/screen-container-bottom'
import ScreenHeadingV2 from '@ui/components/screen-heading-v2'
import CircleButton from '@ui/components/circle-button'
import IconRightHalfArrow from '@icons/IconRightHalfArrow'
import IconThumbUp from '@icons/IconThumbUp'
import { useNavigate } from 'react-router-dom'
import { useTranslation } from '@src/app/domain/administration/stores'

const Container = styled.div`
  display: flex;
  justify-content: center;
  align-items: center;
  height: 100%;
  flex-grow: 100;

  gap: 2rem;
  padding: 6rem;
  position: relative;

  & .content {
    display: flex;
    flex-direction: column;
    gap: 2rem;
  }

  & .action {
    padding-top: 2rem;
  }
`

const PackageSentScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  return (
    <ScreenContainerBottom>
      <Container>
        <div className="content">
          <ScreenHeadingV2
            top={() => <IconThumbUp size="xl" color="var(--primary-light)" />}
            middle={t('Your parcel was sent successfully')}
            bottom={() => (
              <>
                {t(
                  'To complete the payment, please follow the instructions on the payment terminal below.'
                )}
              </>
            )}
          />

          <div className="action">
            <CircleButton
              size="l"
              color="inverted"
              icon={(props) => <IconRightHalfArrow {...props} />}
              textRight={t('Back to home screen')}
              disabled={false}
              onClick={() => navigate('/parcel-locker/idle')}
            />
          </div>
        </div>
      </Container>
    </ScreenContainerBottom>
  )
}

export default PackageSentScreen
