import styled from '@emotion/styled'
import ScreenContainerBottom from '@ui/layouts/screen-container-bottom'
import ScreenHeadingV2 from '@ui/components/screen-heading-v2'
import IconReceipt from '@icons/IconReceipt'
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
`

const TakeReceiptScreen = () => {
  const navigate = useNavigate()
  const { t } = useTranslation()

  return (
    <ScreenContainerBottom>
      <Container onClick={() => navigate('/parcel-locker/package-sent')}>
        <ScreenHeadingV2
          top={() => <IconReceipt size="xl" color="var(--primary-light)" />}
          middle={t('Please take your receipt')}
          bottom={() => (
            <>{t('Keep the receipt as confirmation until parcel is delivered to the recipient.')}</>
          )}
        />
      </Container>
    </ScreenContainerBottom>
  )
}

export default TakeReceiptScreen
